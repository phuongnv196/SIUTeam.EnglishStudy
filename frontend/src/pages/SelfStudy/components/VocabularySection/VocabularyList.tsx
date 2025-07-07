import React, { useState, useRef } from 'react'
import { Card, List, Button, Tag, Space, Typography, message } from 'antd'
import { 
  PlayCircleOutlined, 
  SoundOutlined, 
  EyeOutlined,
  CheckCircleOutlined,
  BookOutlined,
  ApiOutlined,
  LoadingOutlined,
  PhoneOutlined,
  StopOutlined,
  FileTextOutlined
} from '@ant-design/icons'
import type { VocabularyItem } from '../../../../services/vocabularyService'
import textToSpeechService from '../../../../services/textToSpeechService'
import { useSpeakingHub } from '../../../../hooks/useSpeakingHub'

const { Text, Paragraph } = Typography

interface VocabularyListProps {
  currentTopic: { key: string; label: string; color: string; progress: number } | undefined
  filteredVocab: VocabularyItem[]
  onStartFlashcardStudy: (studyType: 'all' | 'unlearned') => void
  onViewWord: (word: VocabularyItem) => void
  onMarkAsLearned: (item: VocabularyItem) => void
}

const VocabularyList: React.FC<VocabularyListProps> = ({
  currentTopic,
  filteredVocab,
  onStartFlashcardStudy,
  onViewWord,
  onMarkAsLearned
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
  if (!currentTopic) return null

  return (
    <Card 
      title={
        <Space>
          <BookOutlined style={{ color: currentTopic.color }} />
          <span>{currentTopic.label} Vocabulary</span>
          <Tag color={currentTopic.color}>
            {filteredVocab.filter(v => v.learned).length}/{filteredVocab.length} đã học
          </Tag>
        </Space>
      }
      extra={
        <Space>
          <Button 
            type="primary" 
            icon={<PlayCircleOutlined />}
            onClick={() => onStartFlashcardStudy('all')}
            disabled={filteredVocab.length === 0}
          >
            Học Flashcard
          </Button>
          <Button 
            icon={<FileTextOutlined />}
            onClick={() => onStartFlashcardStudy('unlearned')}
            disabled={filteredVocab.filter(v => !v.learned).length === 0}
          >
            Học từ chưa biết
          </Button>
        </Space>
      }
    >
      <List
        dataSource={filteredVocab}
        renderItem={(item) => (
          <List.Item
            actions={[
              <Button 
                key="pronunciation"
                type="text" 
                icon={loadingPronunciation === item.id ? <LoadingOutlined /> : <SoundOutlined />}
                onClick={() => handlePlayPronunciation(item.word, item.id)}
                loading={loadingPronunciation === item.id}
              >
                Phát âm
              </Button>,
              <Button 
                key="view"
                type="text" 
                icon={<EyeOutlined />}
                onClick={() => onViewWord(item)}
              >
                Xem chi tiết
              </Button>
            ]}
          >
            <List.Item.Meta
              avatar={
                <div 
                  style={{ 
                    width: '20px', 
                    height: '20px', 
                    borderRadius: '50%',
                    background: item.learned ? '#52c41a' : 'transparent',
                    border: item.learned ? 'none' : '2px solid #d9d9d9',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    cursor: 'pointer'
                  }}
                  onClick={() => onMarkAsLearned(item)}
                >
                  {item.learned && <CheckCircleOutlined style={{ color: 'white', fontSize: '12px' }} />}
                </div>
              }
              title={
                <Space>
                  <Text strong style={{ fontSize: '16px' }}>{item.word}</Text>
                  <Text type="secondary" style={{ fontSize: '12px' }}>
                    {item.pronunciation}
                  </Text>
                  <Tag color={item.level === 'Advanced' ? 'red' : 'blue'}>
                    {item.level}
                  </Tag>
                </Space>
              }
              description={
                <div>
                  <Paragraph style={{ margin: 0, marginBottom: '4px' }}>
                    <Text strong>Nghĩa: </Text>{item.meaning}
                  </Paragraph>
                  <Paragraph style={{ margin: 0 }} type="secondary">
                    <Text italic>"{item.example}"</Text>
                  </Paragraph>
                </div>
              }
            />
          </List.Item>
        )}
      />
    </Card>
  )
}

export default VocabularyList
