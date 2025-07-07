import React from 'react'
import { Modal, Space, Card, Button, Alert, Tag, Typography, Spin } from 'antd'
import { ExperimentOutlined } from '@ant-design/icons'
import textToSpeechService from '../../../../services/textToSpeechService'
import { useSpeakingHub } from '../../../../hooks/useSpeakingHub'

const { Text } = Typography

interface TestModalProps {
  visible: boolean
  onClose: () => void
}

const TestModal: React.FC<TestModalProps> = ({
  visible,
  onClose
}) => {
  // Use speaking hub internally
  const {
    isConnected,
    isConnecting,
    getPronunciationFeedback,
    pronunciationFeedback,
    clearPronunciationFeedback,
    createTalkingAvatar,
    talkingAvatarData,
    clearTalkingAvatarData
  } = useSpeakingHub()

  // Check Python API health internally
  const [pythonApiHealthy, setPythonApiHealthy] = React.useState<boolean | null>(null)

  React.useEffect(() => {
    const checkHealth = async () => {
      const isHealthy = await textToSpeechService.checkPythonAPIHealth()
      setPythonApiHealthy(isHealthy)
    }
    checkHealth()
  }, [])

  const handleClose = () => {
    onClose()
    clearPronunciationFeedback()
    clearTalkingAvatarData()
  }

  const handleGetIPAPronunciation = async (word: string) => {
    await getPronunciationFeedback(word)
  }

  const handleCreateAvatar = async (text: string) => {
    await createTalkingAvatar(text, 3.0, 30)
  }

  return (
    <Modal
      title={
        <Space>
          <ExperimentOutlined />
          <span>Test Speaking Features</span>
        </Space>
      }
      open={visible}
      onCancel={handleClose}
      footer={[
        <Button key="close" onClick={handleClose}>
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
  )
}

export default TestModal
