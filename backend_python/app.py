from flask import Flask, request, jsonify, render_template, abort
from faster_whisper import WhisperModel
import os
import tempfile
import uuid
from werkzeug.utils import secure_filename
import time
from g2p_en import G2p
from viseme_system import VisemeMapper

app = Flask(__name__)

# Cấu hình
app.config['MAX_CONTENT_LENGTH'] = 16 * 1024 * 1024  # 16MB max file size
ALLOWED_EXTENSIONS = {'wav', 'mp3', 'm4a', 'ogg', 'flac', 'aac'}

# Load Faster Whisper model - nhanh hơn nhiều so với whisper thường
print("Loading Faster Whisper model...")
# Sử dụng CPU hoặc GPU
model_size = "base"  # tiny, base, small, medium, large
model = WhisperModel(model_size, device="cpu", compute_type="int8")
print(f"Faster Whisper {model_size} model loaded successfully!")

# Load G2P models for IPA conversion
print("Loading G2P models for IPA conversion...")
try:
    g2p = G2p()  # English G2P model
    print("G2P-EN model loaded successfully!")
    G2P_AVAILABLE = True
except Exception as e:
    print(f"Error loading G2P model: {e}")
    g2p = None
    G2P_AVAILABLE = False

# Skip epitran for now due to encoding issues
EPITRAN_AVAILABLE = False
epitran_eng = None
print("Epitran disabled due to compatibility issues, using G2P-EN only")

# Initialize Viseme Mapper for facial animation
print("Loading Viseme Mapper for facial animation...")
try:
    viseme_mapper = VisemeMapper()
    VISEME_AVAILABLE = True
    print("Viseme Mapper loaded successfully!")
except Exception as e:
    print(f"Error loading Viseme Mapper: {e}")
    viseme_mapper = None
    VISEME_AVAILABLE = False


except Exception as e:
    print(f"Error loading Real Face Animator: {e}")
    real_face_animator = None
    REAL_FACE_AVAILABLE = False

def allowed_file(filename):
    """Kiểm tra file có phải định dạng được phép không"""
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

def arpabet_to_ipa(arpabet_symbols):
    """Chuyển đổi ARPAbet symbols sang IPA với trọng âm"""
    # Mapping ARPAbet base to IPA (không có stress numbers)
    arpabet_to_ipa_map = {
        # Vowels
        'AA': 'ɑː', 'AE': 'æ', 'AH': 'ʌ', 'AO': 'ɔ:', 'AW': 'aʊ', 'AY': 'aɪ',
        'EH': 'ɛ', 'ER': 'ɜː', 'EY': 'eɪ', 'IH': 'ɪ', 'IY': 'i:', 'OW': 'oʊ',
        'OY': 'ɔɪ', 'UH': 'ʊ', 'UW': 'u:',
        
        # Consonants
        'B': 'b', 'CH': 'tʃ', 'D': 'd', 'DH': 'ð', 'F': 'f',
        'G': 'ɡ', 'HH': 'h', 'JH': 'dʒ', 'K': 'k', 'L': 'l',
        'M': 'm', 'N': 'n', 'NG': 'ŋ', 'P': 'p', 'R': 'r',
        'S': 's', 'SH': 'ʃ', 'T': 't', 'TH': 'θ', 'V': 'v',
        'W': 'w', 'Y': 'j', 'Z': 'z', 'ZH': 'ʒ'
    }
    
    ipa_symbols = []
    i = 0
    
    while i < len(arpabet_symbols):
        symbol = arpabet_symbols[i]
        
        # Handle spaces and word boundaries
        if symbol == ' ':
            ipa_symbols.append(' ')
            i += 1
            continue
        
        # Extract base phoneme and stress level
        base_phoneme = symbol
        stress_level = ''
        
        # Check if symbol has stress marker (0, 1, 2)
        if len(symbol) > 1 and symbol[-1].isdigit():
            base_phoneme = symbol[:-1]
            stress_level = symbol[-1]
        
        # Convert base phoneme to IPA
        if base_phoneme in arpabet_to_ipa_map:
            ipa_phone = arpabet_to_ipa_map[base_phoneme]
            
            # Add stress markers for vowels only
            if stress_level and base_phoneme in ['AA', 'AE', 'AH', 'AO', 'AW', 'AY', 'EH', 'ER', 'EY', 'IH', 'IY', 'OW', 'OY', 'UH', 'UW']:
                if stress_level == '1':  # Primary stress
                    ipa_phone = 'ˈ' + ipa_phone
                elif stress_level == '2':  # Secondary stress
                    ipa_phone = 'ˌ' + ipa_phone
                # stress_level == '0' means no stress (unstressed), no marker needed
            
            # Special handling for AH - unstressed AH becomes schwa
            if base_phoneme == 'AH' and stress_level == '0':
                ipa_phone = 'ə'
            elif base_phoneme == 'ER' and stress_level == '0':
                ipa_phone = 'ɚ'  # unstressed ER
            
            ipa_symbols.append(ipa_phone)
        else:
            # Fallback for unknown symbols
            ipa_symbols.append(symbol.lower())
        
        i += 1
    
    return ''.join(ipa_symbols)

def text_to_ipa(text):
    """Chuyển đổi text sang IPA sử dụng G2P-EN"""
    try:
        print(f"Converting text to IPA: '{text[:50]}...'")
        result = {'success': True}
        
        # Method 1: Sử dụng G2P-EN (tốt cho English)
        if G2P_AVAILABLE and g2p is not None:
            try:
                # G2P works with the entire text
                g2p_result = g2p(text)
                print(f"G2P ARPAbet result: {g2p_result}")
                
                # Convert ARPAbet to IPA
                ipa_result = arpabet_to_ipa(g2p_result)
                result['g2p_ipa'] = ipa_result
                result['arpabet'] = ' '.join(g2p_result)  # Also include ARPAbet for reference
                print(f"IPA conversion successful: {ipa_result}")
                
            except Exception as e:
                print(f"G2P conversion error: {e}")
                result['g2p_ipa'] = text  # Fallback to original text
                result['arpabet'] = text
        else:
            print("G2P not available, using original text")
            result['g2p_ipa'] = text  # Fallback to original text
            result['arpabet'] = text
        
        # For now, just copy G2P result to epitran field
        result['epitran_ipa'] = result['g2p_ipa']
        
        return result
        
    except Exception as e:
        print(f"Text to IPA conversion error: {e}")
        return {
            'g2p_ipa': text,  # Fallback to original text
            'epitran_ipa': text,  # Fallback to original text
            'arpabet': text,
            'success': False,
            'error': str(e)
        }

@app.route('/')
def index():
    """Endpoint chính"""
    return render_template('faster_whisper.html')

@app.route('/transcribe', methods=['POST'])
def transcribe_audio():
    """Endpoint để upload file audio và trả về kết quả transcription nhanh"""
    try:
        if 'file' not in request.files:
            return jsonify({'error': 'No file part'}), 400
        
        file = request.files['file']
        
        if file.filename == '':
            return jsonify({'error': 'No selected file'}), 400
        
        if not allowed_file(file.filename):
            return jsonify({
                'error': 'File format not supported',
                'supported_formats': list(ALLOWED_EXTENSIONS)
            }), 400        # Đo thời gian xử lý
        start_time = time.time()
        
        # Tạo tên file tạm với UUID
        file_extension = os.path.splitext(file.filename)[1]
        uid = str(uuid.uuid4()) + file_extension
        temp_file_path = os.path.join(tempfile.gettempdir(), uid)
        
        # Lưu file với tên UUID
        file.save(temp_file_path)
        print(f"Saved temp file: {temp_file_path}")
        
        try:
            print(f"Transcribing file: {file.filename}")
              # Sử dụng Faster Whisper
            segments, info = model.transcribe(temp_file_path, beam_size=5)
            
            # Tổng hợp text từ các segments
            full_text = ""
            segment_list = []
            
            for segment in segments:
                full_text += segment.text
                segment_list.append({
                    "id": segment.id,
                    "start": segment.start,
                    "end": segment.end,
                    "text": segment.text                })
            
            processing_time = time.time() - start_time
              # Chuyển đổi text sang IPA
            print("Converting text to IPA...")
            ipa_result = text_to_ipa(full_text.strip())
            
            response = {
                'success': True,
                'filename': secure_filename(file.filename),
                'text': full_text.strip(),
                'language': info.language,
                'language_probability': info.language_probability,
                'duration': info.duration,
                'processing_time': round(processing_time, 2),
                'segments': segment_list,
                'model': f'faster-whisper-{model_size}',
                'ipa': ipa_result
            }
            
            print(f"Transcription completed in {processing_time:.2f}s")
            return jsonify(response)
            
        finally:
            if os.path.exists(temp_file_path):
                os.unlink(temp_file_path)
    
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/transcribe_chunk', methods=['POST'])
def transcribe_chunk():
    """Endpoint để xử lý audio chunks nhỏ cho real-time"""
    try:
        if 'chunk' not in request.files:
            return jsonify({'error': 'No chunk provided'}), 400
        
        chunk = request.files['chunk']
        start_time = time.time()
        
        # Tạo tên file tạm với UUID
        uid = str(uuid.uuid4()) + '.wav'
        temp_file_path = os.path.join(tempfile.gettempdir(), uid)
        
        # Lưu chunk với tên UUID
        chunk.save(temp_file_path)
        print(f"Saved temp chunk: {temp_file_path}")
        
        try:
            # Xử lý chunk nhỏ với beam_size nhỏ hơn để tăng tốc
            segments, info = model.transcribe(temp_file_path, beam_size=1)
            
            text = ""
            for segment in segments:
                text += segment.text
            
            processing_time = time.time() - start_time
            
            # Chuyển đổi text sang IPA cho chunk
            ipa_result = text_to_ipa(text.strip()) if text.strip() else {'g2p_ipa': '', 'epitran_ipa': '', 'success': True}
            
            return jsonify({
                'success': True,
                'text': text.strip(),
                'processing_time': round(processing_time, 3),
                'language': info.language,
                'ipa': ipa_result
            })
            
        finally:
            if os.path.exists(temp_file_path):
                os.unlink(temp_file_path)
    
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)        }), 500

@app.route('/text_to_ipa', methods=['POST'])
def convert_text_to_ipa():
    """Endpoint để chuyển đổi text sang IPA"""
    try:
        data = request.get_json()
        if not data or 'text' not in data:
            return jsonify({'error': 'No text provided'}), 400
        
        text = data['text']
        if not text.strip():
            return jsonify({'error': 'Empty text provided'}), 400
        
        # Chuyển đổi text sang IPA
        ipa_result = text_to_ipa(text.strip())
        
        return jsonify({
            'success': True,
            'original_text': text,
            'ipa': ipa_result
        })
        
    except Exception as e:
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/health', methods=['GET'])
def health_check():
    """Health check endpoint"""
    return jsonify({
        'status': 'healthy',
        'model': f'faster-whisper-{model_size}',
        'device': 'cpu',
        'features': {
            'speech_recognition': True,
            'ipa_conversion': G2P_AVAILABLE,
            'viseme_mapping': VISEME_AVAILABLE,
            'real_face_animation': REAL_FACE_AVAILABLE
        }
    })

@app.route('/create_talking_avatar', methods=['POST'])
def create_talking_avatar():
    """Tạo animation data cho talking avatar từ text hoặc audio"""
    try:
        # Lấy input từ form hoặc JSON
        if request.is_json:
            data = request.get_json()
            text = data.get('text', '')
            duration = float(data.get('duration', 3.0))
            fps = int(data.get('fps', 30))
        else:
            text = request.form.get('text', '')
            duration = float(request.form.get('duration', 3.0))
            fps = int(request.form.get('fps', 30))
        
        if not text.strip():
            return jsonify({'error': 'No text provided'}), 400
        
        if not VISEME_AVAILABLE:
            return jsonify({'error': 'Viseme system not available'}), 500
        
        print(f"Creating talking avatar for text: '{text[:50]}...'")
        start_time = time.time()
        
        # Step 1: Convert text to IPA
        ipa_result = text_to_ipa(text.strip())
        
        if not ipa_result['success']:
            return jsonify({
                'success': False,
                'error': f'IPA conversion failed: {ipa_result.get("error", "Unknown error")}'
            }), 500
        
        ipa_text = ipa_result['g2p_ipa']
        
        # Step 2: Convert IPA to animation data
        animation_data = viseme_mapper.export_animation_data(ipa_text, duration, fps)
        
        if not animation_data['success']:
            return jsonify({
                'success': False,
                'error': f'Animation generation failed: {animation_data.get("error", "Unknown error")}'
            }), 500
        
        processing_time = time.time() - start_time
        
        # Prepare response
        response = {
            'success': True,
            'text': text,
            'ipa_text': ipa_text,
            'arpabet': ipa_result.get('arpabet', ''),
            'processing_time': round(processing_time, 3),
            'animation': {
                'duration': animation_data['duration'],
                'fps': animation_data['fps'],
                'total_frames': animation_data['total_frames'],
                'visemes_count': len(animation_data['visemes']),
                'phonemes_count': animation_data['phonemes_count'],
                'keyframes': animation_data['keyframes'][:100],  # Limit keyframes for response size
                'visemes': animation_data['visemes']
            }
        }
        
        print(f"Talking avatar created in {processing_time:.3f}s - {len(animation_data['keyframes'])} frames")
        
        return jsonify(response)
        
    except Exception as e:
        print(f"Error creating talking avatar: {e}")
        return jsonify({
            'success': False,
            'error': str(e)
        }), 500

@app.route('/talking_avatar_demo')
def talking_avatar_demo():
    """Demo page cho talking avatar"""
    return render_template('talking_avatar.html')

if __name__ == "__main__":
    app.run(debug=True, host='0.0.0.0', port=5000)
    print("Starting Flask app...")