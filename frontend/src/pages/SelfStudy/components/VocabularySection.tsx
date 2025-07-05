import React, { useState, useEffect, useRef } from 'react'
import { Card, Row, Col, Button, Progress, Tag, List, Typography, Modal, Space, Alert, Spin, message } from 'antd'
import { 
  PlayCircleOutlined, 
  SoundOutlined, 
  EyeOutlined,
  CheckCircleOutlined,
  BookOutlined,
  ApiOutlined,
  ExperimentOutlined,
  LoadingOutlined,
  PhoneOutlined,
  StopOutlined
} from '@ant-design/icons'
import { useSpeakingHub } from '../../../hooks/useSpeakingHub'
import textToSpeechService from '../../../services/textToSpeechService'

const { Title, Paragraph, Text } = Typography

interface VocabularyItem {
  id: number
  word: string
  pronunciation: string
  meaning: string
  example: string
  level: string
  topic: string
  learned: boolean
}

const VocabularySection: React.FC = () => {
  const [selectedTopic, setSelectedTopic] = useState('business')
  const [viewModalVisible, setViewModalVisible] = useState(false)
  const [selectedWord, setSelectedWord] = useState<VocabularyItem | null>(null)
  const [testModalVisible, setTestModalVisible] = useState(false)
  const [loadingPronunciation, setLoadingPronunciation] = useState<number | null>(null)
  const [pythonApiHealthy, setPythonApiHealthy] = useState<boolean | null>(null)
  const [_isRecording, setIsRecording] = useState(false)
  const [recordingWordId, setRecordingWordId] = useState<number | null>(null)
  const [lastTranscription, setLastTranscription] = useState<string>('')
  
  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const audioChunksRef = useRef<Blob[]>([])

  // Speaking Hub integration
  const {
    isConnected,
    isConnecting,
    connect,
    disconnect,
    startSpeakingSession,
    endSpeakingSession,
    getPronunciationFeedback,
    pronunciationFeedback,
    clearPronunciationFeedback,
    createTalkingAvatar,
    talkingAvatarData,
    clearTalkingAvatarData,
    activeSessionId,
    lastTranscription: hubTranscription
  } = useSpeakingHub()

  // Check Python API health on component mount
  useEffect(() => {
    const checkHealth = async () => {
      const isHealthy = await textToSpeechService.checkPythonAPIHealth()
      setPythonApiHealthy(isHealthy)
    }
    checkHealth()
  }, [])

  const topics = [
    { key: 'business', label: 'Business', color: '#1890ff', progress: 75 },
    { key: 'travel', label: 'Travel', color: '#52c41a', progress: 85 },
    { key: 'technology', label: 'Technology', color: '#fa8c16', progress: 60 },
    { key: 'daily', label: 'Daily Life', color: '#eb2f96', progress: 90 },
    { key: 'academic', label: 'Academic', color: '#722ed1', progress: 45 }
  ]

  const vocabularyData: VocabularyItem[] = [
    {
      id: 1,
      word: 'entrepreneur',
      pronunciation: '/ˌɒntrəprəˈnɜː(r)/',
      meaning: 'Doanh nhân, người khởi nghiệp',
      example: 'She is a successful entrepreneur who started three companies.',
      level: 'Advanced',
      topic: 'business',
      learned: true
    },
    {
      id: 2,
      word: 'innovative',
      pronunciation: '/ˈɪnəveɪtɪv/',
      meaning: 'Sáng tạo, đổi mới',
      example: 'The company is known for its innovative products.',
      level: 'Intermediate',
      topic: 'business',
      learned: false
    },
    {
      id: 3,
      word: 'collaboration',
      pronunciation: '/kəˌlæbəˈreɪʃn/',
      meaning: 'Sự hợp tác, cộng tác',
      example: 'The project was successful due to great collaboration.',
      level: 'Intermediate',
      topic: 'business',
      learned: true
    },
    {
      id: 4,
      word: 'itinerary',
      pronunciation: '/aɪˈtɪnərəri/',
      meaning: 'Lịch trình du lịch',
      example: 'We need to plan our itinerary for the Europe trip.',
      level: 'Intermediate',
      topic: 'travel',
      learned: false
    }
  ]

  const currentTopic = topics.find(t => t.key === selectedTopic)
  const filteredVocab = vocabularyData.filter(item => item.topic === selectedTopic)

  const handleViewWord = (word: VocabularyItem) => {
    setSelectedWord(word)
    setViewModalVisible(true)
  }

  const playPronunciation = async (word: string, wordId: number) => {
    setLoadingPronunciation(wordId)
    try {
      // Use browser speech synthesis first
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

  const handleCreateAvatar = async (text: string) => {
    if (!isConnected) {
      await connect()
    }
    
    if (isConnected) {
      await createTalkingAvatar(text, 3.0, 30)
    }
  }

  // Speaking test function
  const handleSpeakingTest = async (word: string, wordId: number) => {
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
      await startSpeakingSession(wordId.toString(), word)
      
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
          stopSpeakingTest()
        }
      }, 5000)
      
    } catch (error) {
      console.error('Error starting speaking test:', error)
      message.error('Không thể truy cập microphone. Vui lòng kiểm tra quyền truy cập.')
      setRecordingWordId(null)
      setIsRecording(false)
    }
  }

  const stopSpeakingTest = () => {
    if (mediaRecorderRef.current?.state === 'recording') {
      mediaRecorderRef.current.stop()
      message.info('Đã dừng ghi âm. Đang xử lý...')
    }
  }

  // Update transcription when hub returns result
  useEffect(() => {
    if (hubTranscription) {
      setLastTranscription(hubTranscription)
      message.success(`Kết quả nhận dạng: "${hubTranscription}"`)
    }
  }, [hubTranscription])

  return (
    <div>
      {/* API Status Section */}
      <Row gutter={[16, 16]} style={{ marginBottom: '16px' }}>
        <Col span={24}>
          <Alert
            message={
              <Space>
                <ApiOutlined />
                <span>API Status</span>
              </Space>
            }
            description={
              <div>
                <div>Python API: {pythonApiHealthy === null ? 'Checking...' : pythonApiHealthy ? '✅ Healthy' : '❌ Offline'}</div>
                <div>SignalR Hub: {isConnecting ? 'Connecting...' : isConnected ? '✅ Connected' : '❌ Disconnected'}</div>
                <div style={{ marginTop: '8px' }}>
                  <Space>
                    <Button 
                      size="small" 
                      onClick={() => connect()}
                      loading={isConnecting}
                      disabled={isConnected}
                    >
                      Connect SignalR
                    </Button>
                    <Button 
                      size="small" 
                      onClick={() => disconnect()}
                      disabled={!isConnected}
                    >
                      Disconnect
                    </Button>
                    <Button 
                      size="small" 
                      type="primary"
                      onClick={() => setTestModalVisible(true)}
                    >
                      <ExperimentOutlined />
                      Test Features
                    </Button>
                  </Space>
                </div>
              </div>
            }
            type={pythonApiHealthy && isConnected ? 'success' : 'warning'}
            showIcon
            style={{ marginBottom: '16px' }}
          />
        </Col>
      </Row>

      {/* Speaking Test Results */}
      {lastTranscription && (
        <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
          <Col span={24}>
            <Alert
              message="🎤 Kết quả nhận dạng giọng nói"
              description={
                <div>
                  <Space direction="vertical" style={{ width: '100%' }}>
                    <div>
                      <Text strong>Từ được nhận dạng: </Text>
                      <Text code style={{ fontSize: '16px', color: '#1890ff' }}>{lastTranscription}</Text>
                    </div>
                    {pronunciationFeedback && (
                      <div>
                        <Text strong>IPA: </Text>
                        <Text code>{pronunciationFeedback.ipaNotation}</Text>
                        <Text strong style={{ marginLeft: '16px' }}>ARPAbet: </Text>
                        <Text code>{pronunciationFeedback.arpabet}</Text>
                      </div>
                    )}
                  </Space>
                </div>
              }
              type="success"
              showIcon
              closable
              onClose={() => {
                setLastTranscription('')
                clearPronunciationFeedback()
              }}
            />
          </Col>
        </Row>
      )}

      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>📚 Chọn chủ đề từ vựng</Title>
          <Row gutter={[12, 12]}>
            {topics.map(topic => (
              <Col key={topic.key} xs={24} sm={12} md={8} lg={6}>
                <Card 
                  size="small"
                  className={selectedTopic === topic.key ? 'selected-topic' : ''}
                  style={{ 
                    cursor: 'pointer',
                    border: selectedTopic === topic.key ? `2px solid ${topic.color}` : '1px solid #d9d9d9'
                  }}
                  onClick={() => setSelectedTopic(topic.key)}
                >
                  <div style={{ textAlign: 'center' }}>
                    <Text strong style={{ color: topic.color }}>{topic.label}</Text>
                    <Progress 
                      percent={topic.progress} 
                      size="small" 
                      strokeColor={topic.color}
                      style={{ marginTop: '8px' }}
                    />
                  </div>
                </Card>
              </Col>
            ))}
          </Row>
        </Col>
      </Row>

      {currentTopic && (
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
            <Button type="primary" icon={<PlayCircleOutlined />}>
              Bắt đầu học
            </Button>
          }
        >
          <List
            dataSource={filteredVocab}
            renderItem={(item) => (
              <List.Item
                actions={[
                  <Button 
                    type="text" 
                    icon={loadingPronunciation === item.id ? <LoadingOutlined /> : <SoundOutlined />}
                    onClick={() => playPronunciation(item.word, item.id)}
                    loading={loadingPronunciation === item.id}
                  >
                    Phát âm
                  </Button>,
                  <Button 
                    type="text" 
                    icon={<ApiOutlined />}
                    onClick={() => handleGetIPAPronunciation(item.word)}
                    disabled={!isConnected}
                  >
                    IPA
                  </Button>,
                  <Button 
                    type="text" 
                    icon={recordingWordId === item.id ? <StopOutlined /> : <PhoneOutlined />}
                    onClick={() => recordingWordId === item.id ? stopSpeakingTest() : handleSpeakingTest(item.word, item.id)}
                    disabled={!isConnected || (recordingWordId !== null && recordingWordId !== item.id)}
                    loading={recordingWordId === item.id}
                    danger={recordingWordId === item.id}
                  >
                    {recordingWordId === item.id ? 'Dừng' : 'Test Nói'}
                  </Button>,
                  <Button 
                    type="text" 
                    icon={<EyeOutlined />}
                    onClick={() => handleViewWord(item)}
                  >
                    Xem chi tiết
                  </Button>
                ]}
              >
                <List.Item.Meta
                  avatar={
                    item.learned ? 
                    <CheckCircleOutlined style={{ color: '#52c41a', fontSize: '20px' }} /> :
                    <div style={{ 
                      width: '20px', 
                      height: '20px', 
                      border: '2px solid #d9d9d9', 
                      borderRadius: '50%' 
                    }} />
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
      )}

      <Modal
        title={selectedWord?.word}
        open={viewModalVisible}
        onCancel={() => setViewModalVisible(false)}
        footer={[
          <Button key="close" onClick={() => setViewModalVisible(false)}>
            Đóng
          </Button>,
          <Button 
            key="ipa" 
            icon={<ApiOutlined />}
            onClick={() => selectedWord && handleGetIPAPronunciation(selectedWord.word)}
            disabled={!isConnected}
          >
            Get IPA
          </Button>,
          <Button 
            key="speaking" 
            icon={<PhoneOutlined />}
            onClick={() => selectedWord && handleSpeakingTest(selectedWord.word, selectedWord.id)}
            disabled={!isConnected || recordingWordId === selectedWord?.id}
            loading={recordingWordId === selectedWord?.id}
          >
            Test Speaking
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
                  onClick={() => selectedWord && playPronunciation(selectedWord.word, selectedWord.id)}
                  style={{ marginLeft: '8px' }}
                />
                <Button 
                  type="text" 
                  icon={recordingWordId === selectedWord.id ? <StopOutlined /> : <PhoneOutlined />}
                  onClick={() => recordingWordId === selectedWord.id ? stopSpeakingTest() : handleSpeakingTest(selectedWord.word, selectedWord.id)}
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

      {/* Test Modal */}
      <Modal
        title={
          <Space>
            <ExperimentOutlined />
            <span>Test Speaking Features</span>
          </Space>
        }
        open={testModalVisible}
        onCancel={() => {
          setTestModalVisible(false)
          clearPronunciationFeedback()
          clearTalkingAvatarData()
        }}
        footer={[
          <Button key="close" onClick={() => setTestModalVisible(false)}>
            Close
          </Button>
        ]}
        width={800}
      >
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          {/* Connection Status */}
          <Card size="small" title="Connection Status">
            <Space direction="vertical" style={{ width: '100%' }}>
              <div>
                <Text strong>SignalR: </Text>
                <Tag color={isConnected ? 'green' : 'red'}>
                  {isConnected ? 'Connected' : 'Disconnected'}
                </Tag>
                {isConnecting && <Spin size="small" style={{ marginLeft: '8px' }} />}
              </div>
              <div>
                <Text strong>Python API: </Text>
                <Tag color={pythonApiHealthy ? 'green' : 'red'}>
                  {pythonApiHealthy ? 'Healthy' : 'Offline'}
                </Tag>
              </div>
            </Space>
          </Card>

          {/* IPA Pronunciation Test */}
          <Card size="small" title="IPA Pronunciation Test">
            <Space direction="vertical" style={{ width: '100%' }}>
              <Space>
                <Button 
                  onClick={() => handleGetIPAPronunciation('hello')}
                  disabled={!isConnected}
                >
                  Test "hello"
                </Button>
                <Button 
                  onClick={() => handleGetIPAPronunciation('entrepreneur')}
                  disabled={!isConnected}
                >
                  Test "entrepreneur"
                </Button>
                <Button 
                  onClick={() => handleGetIPAPronunciation('collaboration')}
                  disabled={!isConnected}
                >
                  Test "collaboration"
                </Button>
              </Space>
              
              {pronunciationFeedback && (
                <Alert
                  message="Pronunciation Feedback"
                  description={
                    <div>
                      <div><Text strong>Word:</Text> {pronunciationFeedback.originalText}</div>
                      <div><Text strong>IPA:</Text> <Text code>{pronunciationFeedback.ipaNotation}</Text></div>
                      <div><Text strong>ARPAbet:</Text> <Text code>{pronunciationFeedback.arpabet}</Text></div>
                    </div>
                  }
                  type="success"
                  showIcon
                  closable
                  onClose={clearPronunciationFeedback}
                />
              )}
            </Space>
          </Card>

          {/* Talking Avatar Test */}
          <Card size="small" title="Talking Avatar Test">
            <Space direction="vertical" style={{ width: '100%' }}>
              <Space>
                <Button 
                  onClick={() => handleCreateAvatar('Hello world')}
                  disabled={!isConnected}
                >
                  Test "Hello world"
                </Button>
                <Button 
                  onClick={() => handleCreateAvatar('Welcome to English learning')}
                  disabled={!isConnected}
                >
                  Test "Welcome to English learning"
                </Button>
              </Space>
              
              {talkingAvatarData && (
                <Alert
                  message="Talking Avatar Created"
                  description={
                    <div>
                      <div><Text strong>Text:</Text> {talkingAvatarData.text}</div>
                      <div><Text strong>IPA:</Text> <Text code>{talkingAvatarData.ipaText}</Text></div>
                      <div><Text strong>Frames:</Text> {talkingAvatarData.animation.totalFrames}</div>
                      <div><Text strong>Duration:</Text> {talkingAvatarData.animation.duration}s</div>
                      <div><Text strong>Processing Time:</Text> {talkingAvatarData.processingTime}s</div>
                    </div>
                  }
                  type="success"
                  showIcon
                  closable
                  onClose={clearTalkingAvatarData}
                />
              )}
            </Space>
          </Card>

          {/* Browser TTS Test */}
          <Card size="small" title="Browser Text-to-Speech Test">
            <Space>
              <Button onClick={() => textToSpeechService.speakWithBrowser('Hello world')}>
                Test "Hello world"
              </Button>
              <Button onClick={() => textToSpeechService.speakWithBrowser('entrepreneur')}>
                Test "entrepreneur"
              </Button>
              <Button onClick={() => textToSpeechService.stopSpeaking()}>
                Stop Speaking
              </Button>
            </Space>
          </Card>
        </Space>
      </Modal>
    </div>
  )
}

export default VocabularySection
