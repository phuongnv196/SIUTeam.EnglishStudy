// Text-to-Speech Service
export class TextToSpeechService {
  private static instance: TextToSpeechService;
  private pythonApiUrl = 'http://127.0.0.1:5000';
  private synth = window.speechSynthesis;

  private constructor() {}

  public static getInstance(): TextToSpeechService {
    if (!TextToSpeechService.instance) {
      TextToSpeechService.instance = new TextToSpeechService();
    }
    return TextToSpeechService.instance;
  }

  // Method 1: Use browser's built-in Speech Synthesis API
  public speakWithBrowser(text: string, lang: string = 'en-US'): Promise<void> {
    return new Promise((resolve, reject) => {
      if (!this.synth) {
        reject(new Error('Speech synthesis not supported'));
        return;
      }

      // Cancel any ongoing speech
      this.synth.cancel();

      const utterance = new SpeechSynthesisUtterance(text);
      utterance.lang = lang;
      utterance.rate = 0.8;
      utterance.pitch = 1;
      utterance.volume = 1;

      utterance.onend = () => resolve();
      utterance.onerror = (error) => reject(error);

      this.synth.speak(utterance);
    });
  }

  // Method 2: Get IPA pronunciation from Python API
  public async getIPAPronunciation(text: string): Promise<string> {
    try {
      const response = await fetch(`${this.pythonApiUrl}/text_to_ipa`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ text }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const result = await response.json();
      return result.ipa?.g2p_ipa || result.ipa?.epitran_ipa || text;
    } catch (error) {
      console.error('Error getting IPA pronunciation:', error);
      return text; // Fallback to original text
    }
  }

  // Method 3: Check if Python API is available
  public async checkPythonAPIHealth(): Promise<boolean> {
    try {
      const response = await fetch(`${this.pythonApiUrl}/health`, {
        method: 'GET',
      });
      return response.ok;
    } catch (error) {
      console.error('Python API not available:', error);
      return false;
    }
  }

  // Method 4: Stop current speech
  public stopSpeaking(): void {
    if (this.synth) {
      this.synth.cancel();
    }
  }

  // Method 5: Get available voices
  public getAvailableVoices(): SpeechSynthesisVoice[] {
    if (!this.synth) return [];
    return this.synth.getVoices().filter(voice => 
      voice.lang.startsWith('en-') // English voices only
    );
  }

  // Method 6: Speak with specific voice
  public speakWithVoice(text: string, voiceName?: string): Promise<void> {
    return new Promise((resolve, reject) => {
      if (!this.synth) {
        reject(new Error('Speech synthesis not supported'));
        return;
      }

      this.synth.cancel();

      const utterance = new SpeechSynthesisUtterance(text);
      
      if (voiceName) {
        const voices = this.getAvailableVoices();
        const selectedVoice = voices.find(voice => voice.name === voiceName);
        if (selectedVoice) {
          utterance.voice = selectedVoice;
        }
      }

      utterance.rate = 0.8;
      utterance.pitch = 1;
      utterance.volume = 1;

      utterance.onend = () => resolve();
      utterance.onerror = (error) => reject(error);

      this.synth.speak(utterance);
    });
  }
}

export default TextToSpeechService.getInstance();
