import React, { useState, useRef } from 'react'
import { Modal, Space, Typography, Button, Tag, message } from 'antd'
import { 
  SoundOutlined, 
  PhoneOutlined, 
  StopOutlined, 
  PlayCircleOutlined, 
  ApiOutlined 
} from '@ant-design/icons'
import type { VocabularyItem } from '../../../../services/vocabularyService'
import textToSpeechService from '../../../../services/textToSpeechService'
import { useSpeakingHub } from '../../../../hooks/useSpeakingHub'

const { Text } = Typography

interface VocabularyModalProps {
  visible: boolean
  selectedWord: VocabularyItem | null
  onClose: () => void
}

const VocabularyModal: React.FC<VocabularyModalProps> = ({
  visible,
  selectedWord,
  onClose
}) => {
  const [loadingPronunciation, setLoadingPronunciation] = useState<string | null>(null)
  const [recordingWordId, setRecordingWordId] = useState<string | null>(null)
  const [isRecording, setIsRecording] = useState(false)
  
  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const audioChunksRef = useRef<Blob[]>([])

  // Speaking Hub integration
  const {
    isConnected,
    connect,
    startSpeakingSession,
    endSpeakingSession,
    getPronunciationFeedback,
    activeSessionId
  } = useSpeakingHub()

  const handlePlayPronunciation = async (word: string, wordId: string) => {
    setLoadingPronunciation(wordId)
    try {
      // Use browser speech synthesis
      await textToSpeechService.speakWithBrowser(word)
    } catch (error) {
      console.error('Error playing pronunciation:', error)
    } finally {
      setLoadingPronunciation(null)
    }
  }

  const handleGetIPAPronunciation = async (word: string) => {
    if (!isConnected) {
      // Try to connect first
      await connect()
    }
    
    if (isConnected) {
      await getPronunciationFeedback(word)
    }
  }

  // Speaking test function
  const handleSpeakingTest = async (word: string, wordId: string) => {
    if (!isConnected) {
      message.info('Đang kết nối SignalR...')
      await connect()
    }

    if (!isConnected) {
      message.error('Không thể kết nối đến Speaking Hub')
      return
    }

    setRecordingWordId(wordId)
    
    try {
      // Request microphone permission and start recording
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      
      // Start speaking session (lessonId can be wordId, expectedText is the word)
      await startSpeakingSession(wordId, word)
      
      // Setup MediaRecorder for chunked recording
      const mediaRecorder = new MediaRecorder(stream, {
        mimeType: 'audio/webm;codecs=opus'
      })
      
      mediaRecorderRef.current = mediaRecorder
      audioChunksRef.current = []
      
      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          audioChunksRef.current.push(event.data)
        }
      }
      
      mediaRecorder.onstop = async () => {
        const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/webm' })
        
        // Convert blob to base64 string for SignalR
        const reader = new FileReader()
        reader.onloadend = async () => {
          const base64Audio = (reader.result as string).split(',')[1] // Remove data:audio/webm;base64, prefix
          
          // End speaking session with final audio
          if (activeSessionId) {
            await endSpeakingSession(base64Audio, word)
          }
        }
        reader.readAsDataURL(audioBlob)
        
        // Clean up
        stream.getTracks().forEach(track => track.stop())
        setRecordingWordId(null)
        setIsRecording(false)
      }
      
      // Start recording
      setIsRecording(true)
      mediaRecorder.start(1000) // Collect data every 1 second
      
      message.success(`Đang ghi âm "${word}". Nhấn Stop để kết thúc.`)
      
      // Auto stop after 5 seconds
      setTimeout(() => {
        if (mediaRecorderRef.current?.state === 'recording') {
          handleStopSpeakingTest()
        }
      }, 5000)
      
    } catch (error) {
      console.error('Error starting speaking test:', error)
      message.error('Không thể truy cập microphone. Vui lòng kiểm tra quyền truy cập.')
      setRecordingWordId(null)
      setIsRecording(false)
    }
  }

  const handleStopSpeakingTest = () => {
    if (mediaRecorderRef.current?.state === 'recording') {
      mediaRecorderRef.current.stop()
      message.info('Đã dừng ghi âm. Đang xử lý...')
    }
  }
  return (
    <Modal
      title={selectedWord?.word}
      open={visible}
      onCancel={onClose}
      footer={[
        <Button key="close" onClick={onClose}>
          Đóng
        </Button>,
        <Button 
          key="practice" 
          type="primary"
          icon={<PlayCircleOutlined />}
        >
          Luyện tập từ này
        </Button>
      ]}
    >
      {selectedWord && (
        <div>
          <Space direction="vertical" size="middle" style={{ width: '100%' }}>
            <div>
              <Text strong>Phát âm: </Text>
              <Text code>{selectedWord.pronunciation}</Text>
              <Button 
                type="text" 
                icon={<SoundOutlined />}
                onClick={() => selectedWord && handlePlayPronunciation(selectedWord.word, selectedWord.id)}
                loading={loadingPronunciation === selectedWord.id}
                style={{ marginLeft: '8px' }}
              />
              <Button 
                type="text" 
                icon={recordingWordId === selectedWord.id ? <StopOutlined /> : <PhoneOutlined />}
                onClick={() => recordingWordId === selectedWord.id ? handleStopSpeakingTest() : handleSpeakingTest(selectedWord.word, selectedWord.id)}
                disabled={!isConnected || (recordingWordId !== null && recordingWordId !== selectedWord.id)}
                loading={recordingWordId === selectedWord.id}
                danger={recordingWordId === selectedWord.id}
                style={{ marginLeft: '8px' }}
              >
                {recordingWordId === selectedWord.id ? 'Dừng' : 'Test Nói'}
              </Button>
            </div>
            <div>
              <Text strong>Nghĩa: </Text>
              <Text>{selectedWord.meaning}</Text>
            </div>
            <div>
              <Text strong>Ví dụ: </Text>
              <Text italic>"{selectedWord.example}"</Text>
            </div>
            <div>
              <Text strong>Cấp độ: </Text>
              <Tag color={selectedWord.level === 'Advanced' ? 'red' : 'blue'}>
                {selectedWord.level}
              </Tag>
            </div>
          </Space>
        </div>
      )}
    </Modal>
  )
}

export default VocabularyModal
