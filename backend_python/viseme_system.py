"""
Viseme Mapping System - Chuyển đổi IPA phonemes thành mouth shapes
"""
import re
import random
import json

class VisemeMapper:
    def __init__(self):
        # Mapping IPA phonemes to visemes (mouth shapes)
        self.ipa_to_viseme = {
            # Silence/Rest
            'sil': 'rest',
            ' ': 'rest',
            
            # Vowels
            'i:': 'ee',      # FLEECE
            'ɪ': 'ih',       # KIT  
            'ɛ': 'eh',       # DRESS
            'æ': 'ae',       # TRAP
            'ɑː': 'aa',      # PALM
            'ɔ:': 'aw',      # THOUGHT
            'ʊ': 'uh',       # FOOT
            'u:': 'oo',      # GOOSE
            'ʌ': 'ah',       # STRUT
            'ɜː': 'er',      # NURSE
            'ə': 'uh',       # schwa
            'ɚ': 'er',       # r-colored schwa
            
            # Diphthongs
            'eɪ': 'ay',      # FACE
            'aɪ': 'ai',      # PRICE
            'ɔɪ': 'oy',      # CHOICE
            'aʊ': 'ow',      # MOUTH
            'oʊ': 'oh',      # GOAT
            
            # Consonants - Bilabials (lips together)
            'p': 'p',        # pat
            'b': 'p',        # bat
            'm': 'p',        # mat
            
            # Labiodentals (lips to teeth)
            'f': 'f',        # fat
            'v': 'f',        # vat
            
            # Dental/Alveolar
            'θ': 'th',       # think
            'ð': 'th',       # that
            't': 't',        # top
            'd': 't',        # dot
            'n': 't',        # not
            's': 's',        # sit
            'z': 's',        # zip
            'l': 'l',        # lot
            'r': 'r',        # rot
            
            # Post-alveolar
            'ʃ': 'sh',       # shop
            'ʒ': 'sh',       # measure
            'tʃ': 'ch',      # chop
            'dʒ': 'ch',      # job
            
            # Velar
            'k': 'k',        # cat
            'ɡ': 'k',        # got
            'ŋ': 'ng',       # sing
            
            # Glottal
            'h': 'h',        # hat
            
            # Glides/Approximants
            'w': 'w',        # wet
            'j': 'y',        # yet
        }
        
        # Viseme parameters cho mouth animation
        self.viseme_params = {
            'rest': {
                'mouth_open': 0.0, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.0,
                'duration': 0.1
            },
            'aa': {
                'mouth_open': 0.8, 
                'mouth_width': 0.7, 
                'lip_round': 0.0,
                'jaw_open': 0.7,
                'duration': 0.15
            },  # "father"
            'ae': {
                'mouth_open': 0.4, 
                'mouth_width': 0.8, 
                'lip_round': 0.0,
                'jaw_open': 0.4,
                'duration': 0.12
            },  # "cat"
            'ah': {
                'mouth_open': 0.5, 
                'mouth_width': 0.6, 
                'lip_round': 0.0,
                'jaw_open': 0.4,
                'duration': 0.12
            },  # "cut"
            'aw': {
                'mouth_open': 0.6, 
                'mouth_width': 0.4, 
                'lip_round': 0.3,
                'jaw_open': 0.5,
                'duration': 0.15
            },  # "caught"
            'ay': {
                'mouth_open': 0.3, 
                'mouth_width': 0.7, 
                'lip_round': 0.0,
                'jaw_open': 0.2,
                'duration': 0.18
            },  # "say"
            'ee': {
                'mouth_open': 0.2, 
                'mouth_width': 0.9, 
                'lip_round': 0.0,
                'jaw_open': 0.1,
                'duration': 0.12
            },  # "see"
            'eh': {
                'mouth_open': 0.4, 
                'mouth_width': 0.7, 
                'lip_round': 0.0,
                'jaw_open': 0.3,
                'duration': 0.12
            },  # "bed"
            'er': {
                'mouth_open': 0.3, 
                'mouth_width': 0.5, 
                'lip_round': 0.2,
                'jaw_open': 0.2,
                'duration': 0.15
            },  # "bird"
            'ih': {
                'mouth_open': 0.2, 
                'mouth_width': 0.6, 
                'lip_round': 0.0,
                'jaw_open': 0.1,
                'duration': 0.10
            },  # "bit"
            'oo': {
                'mouth_open': 0.3, 
                'mouth_width': 0.3, 
                'lip_round': 0.8,
                'jaw_open': 0.2,
                'duration': 0.12
            },  # "book"
            'uh': {
                'mouth_open': 0.2, 
                'mouth_width': 0.4, 
                'lip_round': 0.2,
                'jaw_open': 0.1,
                'duration': 0.10
            },  # "book"
            'p': {
                'mouth_open': 0.0, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.0,
                'duration': 0.08
            },   # "pat"
            'f': {
                'mouth_open': 0.1, 
                'mouth_width': 0.4, 
                'lip_round': 0.0,
                'jaw_open': 0.0,
                'duration': 0.10
            },   # "fat"  
            't': {
                'mouth_open': 0.1, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.1,
                'duration': 0.08
            },   # "top"
            's': {
                'mouth_open': 0.1, 
                'mouth_width': 0.6, 
                'lip_round': 0.0,
                'jaw_open': 0.0,
                'duration': 0.12
            },   # "sit"
            'sh': {
                'mouth_open': 0.2, 
                'mouth_width': 0.4, 
                'lip_round': 0.4,
                'jaw_open': 0.1,
                'duration': 0.12
            },  # "shop"
            'ch': {
                'mouth_open': 0.2, 
                'mouth_width': 0.4, 
                'lip_round': 0.3,
                'jaw_open': 0.1,
                'duration': 0.10
            },  # "chop"
            'th': {
                'mouth_open': 0.1, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.0,
                'duration': 0.10
            },  # "think"
            'k': {
                'mouth_open': 0.3, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.2,
                'duration': 0.08
            },   # "cat"
            'l': {
                'mouth_open': 0.2, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.1,
                'duration': 0.10
            },   # "lot"
            'r': {
                'mouth_open': 0.2, 
                'mouth_width': 0.4, 
                'lip_round': 0.3,
                'jaw_open': 0.1,
                'duration': 0.12
            },   # "rot"
            'w': {
                'mouth_open': 0.3, 
                'mouth_width': 0.3, 
                'lip_round': 0.7,
                'jaw_open': 0.2,
                'duration': 0.10
            },   # "wet"
            'y': {
                'mouth_open': 0.2, 
                'mouth_width': 0.7, 
                'lip_round': 0.0,
                'jaw_open': 0.1,
                'duration': 0.08
            },   # "yet"
            'h': {
                'mouth_open': 0.4, 
                'mouth_width': 0.6, 
                'lip_round': 0.0,
                'jaw_open': 0.3,
                'duration': 0.10
            },   # "hat"
            'ng': {
                'mouth_open': 0.2, 
                'mouth_width': 0.5, 
                'lip_round': 0.0,
                'jaw_open': 0.1,
                'duration': 0.12
            },  # "sing"
        }
        
        # Eye blink patterns
        self.blink_probability = 0.05  # 5% chance per frame
        self.blink_duration = 0.15     # Blink lasts 150ms
        
    def parse_ipa_phonemes(self, ipa_text):
        """Parse IPA text thành individual phonemes"""
        # Remove stress markers
        clean_ipa = ipa_text.replace('ˈ', '').replace('ˌ', '')
        
        # Simple phoneme parsing - split by spaces and common boundaries
        phonemes = []
        current_phoneme = ""
        
        i = 0
        while i < len(clean_ipa):
            char = clean_ipa[i]
            
            if char == ' ':
                if current_phoneme:
                    phonemes.append(current_phoneme)
                    current_phoneme = ""
                phonemes.append(' ')
            elif char in 'ː':  # Length marker
                current_phoneme += char
                phonemes.append(current_phoneme)
                current_phoneme = ""
            elif char in 'aeiouɑɛɪɔʊʌɜəɚ':  # Vowels
                current_phoneme += char
                # Check for diphthongs
                if i + 1 < len(clean_ipa) and clean_ipa[i + 1] in 'ɪʊ':
                    current_phoneme += clean_ipa[i + 1]
                    i += 1
                phonemes.append(current_phoneme)
                current_phoneme = ""
            else:  # Consonants
                current_phoneme += char
                # Check for digraphs
                if i + 1 < len(clean_ipa):
                    next_char = clean_ipa[i + 1]
                    if (char == 't' and next_char == 'ʃ') or \
                       (char == 'd' and next_char == 'ʒ'):
                        current_phoneme += next_char
                        i += 1
                phonemes.append(current_phoneme)
                current_phoneme = ""
            
            i += 1
        
        if current_phoneme:
            phonemes.append(current_phoneme)
        
        return [p for p in phonemes if p]  # Remove empty strings
    
    def ipa_to_visemes(self, ipa_text, total_duration=3.0):
        """Convert IPA text to viseme sequence with timing"""
        try:
            phonemes = self.parse_ipa_phonemes(ipa_text)
            
            if not phonemes:
                return []
            
            visemes = []
            current_time = 0.0
            
            # Calculate base duration per phoneme
            base_duration = total_duration / len(phonemes)
            
            for phoneme in phonemes:
                if phoneme.strip():
                    # Map phoneme to viseme
                    viseme = self.ipa_to_viseme.get(phoneme, 'rest')
                    params = self.viseme_params.get(viseme, self.viseme_params['rest']).copy()
                    
                    # Adjust duration based on phoneme type
                    duration = base_duration * params.get('duration', 0.1) / 0.1
                    
                    # Generate blink timing
                    should_blink = random.random() < self.blink_probability
                    
                    viseme_data = {
                        'phoneme': phoneme,
                        'viseme': viseme,
                        'start_time': current_time,
                        'end_time': current_time + duration,
                        'duration': duration,
                        'mouth_open': params['mouth_open'],
                        'mouth_width': params['mouth_width'],
                        'lip_round': params['lip_round'],
                        'jaw_open': params['jaw_open'],
                        'blink': should_blink
                    }
                    
                    visemes.append(viseme_data)
                    current_time += duration
            
            return visemes
            
        except Exception as e:
            print(f"Error in IPA to visemes conversion: {e}")
            return []
    
    def generate_animation_keyframes(self, visemes, fps=30):
        """Generate animation keyframes for facial animation"""
        if not visemes:
            return []
        
        keyframes = []
        total_duration = max(v['end_time'] for v in visemes)
        total_frames = int(total_duration * fps)
        
        for frame_num in range(total_frames):
            time_point = frame_num / fps
            
            # Find current viseme
            current_viseme = None
            for viseme in visemes:
                if viseme['start_time'] <= time_point <= viseme['end_time']:
                    current_viseme = viseme
                    break
            
            if current_viseme is None:
                current_viseme = {
                    'mouth_open': 0.0,
                    'mouth_width': 0.5,
                    'lip_round': 0.0,
                    'jaw_open': 0.0,
                    'blink': False
                }
            
            # Generate smooth transitions
            progress = 0.5  # Middle of transition
            if current_viseme:
                duration = current_viseme['end_time'] - current_viseme['start_time']
                if duration > 0:
                    progress = (time_point - current_viseme['start_time']) / duration
            
            # Apply easing for smooth animation
            ease_progress = self.ease_in_out(progress)
            
            keyframe = {
                'frame': frame_num,
                'time': time_point,
                'mouth_open': current_viseme['mouth_open'] * ease_progress,
                'mouth_width': current_viseme['mouth_width'],
                'lip_round': current_viseme['lip_round'],
                'jaw_open': current_viseme['jaw_open'] * ease_progress,
                'blink': current_viseme.get('blink', False) and (frame_num % 10 < 3),  # Blink effect
                'phoneme': current_viseme.get('phoneme', ''),
                'viseme': current_viseme.get('viseme', 'rest')
            }
            
            keyframes.append(keyframe)
        
        return keyframes
    
    def ease_in_out(self, t):
        """Easing function for smooth animations"""
        if t < 0.5:
            return 2 * t * t
        else:
            return -1 + (4 - 2 * t) * t
    
    def export_animation_data(self, ipa_text, duration=3.0, fps=30):
        """Export complete animation data for frontend"""
        try:
            # Convert IPA to visemes
            visemes = self.ipa_to_visemes(ipa_text, duration)
            
            # Generate keyframes
            keyframes = self.generate_animation_keyframes(visemes, fps)
            
            animation_data = {
                'success': True,
                'ipa_text': ipa_text,
                'duration': duration,
                'fps': fps,
                'total_frames': len(keyframes),
                'visemes': visemes,
                'keyframes': keyframes,
                'phonemes_count': len([v for v in visemes if v['phoneme'] != ' '])
            }
            
            return animation_data
            
        except Exception as e:
            return {
                'success': False,
                'error': str(e),
                'ipa_text': ipa_text
            }

# Test the system
if __name__ == "__main__":
    mapper = VisemeMapper()
    
    # Test với text mẫu
    test_ipa = "ˈhɛloʊ wɜːld"
    result = mapper.export_animation_data(test_ipa, duration=2.0)
    
    print("Animation Data:")
    print(f"Success: {result['success']}")
    if result['success']:
        print(f"Total frames: {result['total_frames']}")
        print(f"Visemes count: {len(result['visemes'])}")
        print(f"First few keyframes:")
        for i, kf in enumerate(result['keyframes'][:5]):
            print(f"  Frame {kf['frame']}: mouth_open={kf['mouth_open']:.2f}, viseme={kf['viseme']}")
