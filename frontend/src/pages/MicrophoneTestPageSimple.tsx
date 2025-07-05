import React, { useState, useEffect } from 'react'
import { Card, Button, Switch, Typography, Alert, Space, Row, Col, Statistic, Progress } from 'antd'
import { AudioOutlined, StopOutlined, PlayCircleOutlined } from '@ant-design/icons'
import { useMicrophone } from '../hooks/useMicrophone'

const { Title, Text } = Typography

const MicrophoneTestPage: React.FC = () => {
  const [enhancedNoiseReduction, setEnhancedNoiseReduction] = useState(true) // Default to true
  const [recordedBlob, setRecordedBlob] = useState<Blob | null>(null)
  const [audioUrl, setAudioUrl] = useState<string | null>(null)

  const {
    isRecording,
    isPermissionGranted,
    error,
    audioLevel,
    isEnhancedNoiseReductionEnabled,
    requestPermission,
    startRecording,
    stopRecording,
    cleanup,
    isSupported,
    blobToBase64
  } = useMicrophone({
    enhancedNoiseReduction,
    onDataAvailable: (blob) => {
      setRecordedBlob(blob)
      const url = URL.createObjectURL(blob)
      setAudioUrl(url)
    },
    chunkInterval: 1000
  })

  useEffect(() => {
    return () => {
      cleanup()
      if (audioUrl) {
        URL.revokeObjectURL(audioUrl)
      }
    }
  }, [cleanup, audioUrl])

  const handleStartRecording = async () => {
    if (!isPermissionGranted) {
      await requestPermission()
    }
    await startRecording()
  }

  const handleStopRecording = async () => {
    const blob = await stopRecording()
    if (blob) {
      setRecordedBlob(blob)
      const url = URL.createObjectURL(blob)
      setAudioUrl(url)
    }
  }

  const handlePlayRecording = () => {
    if (audioUrl) {
      const audio = new Audio(audioUrl)
      audio.play()
    }
  }

  const handleDownloadRecording = async () => {
    if (recordedBlob) {
      const base64 = await blobToBase64(recordedBlob)
      console.log('Recording as base64:', base64.substring(0, 100) + '...')
      
      // Download file
      const url = URL.createObjectURL(recordedBlob)
      const a = document.createElement('a')
      a.href = url
      a.download = `recording-${Date.now()}.webm`
      document.body.appendChild(a)
      a.click()
      document.body.removeChild(a)
      URL.revokeObjectURL(url)
    }
  }

  if (!isSupported()) {
    return (
      <Alert
        message="Microphone Not Supported"
        description="Your browser does not support microphone access."
        type="error"
        showIcon
      />
    )
  }

  return (
    <div style={{ padding: '24px', maxWidth: '1200px', margin: '0 auto' }}>
      <Title level={2}>Microphone Test with Browser Noise Reduction</Title>
      
      {error && (
        <Alert
          message="Error"
          description={error}
          type="error"
          showIcon
          style={{ marginBottom: '16px' }}
        />
      )}

      <Row gutter={[16, 16]}>
        {/* Recording Controls */}
        <Col span={12}>
          <Card title="Recording Controls" bordered={false}>
            <Space direction="vertical" style={{ width: '100%' }}>
              <div>
                <Text strong>Permission Status: </Text>
                <Text type={isPermissionGranted ? 'success' : 'danger'}>
                  {isPermissionGranted === null ? 'Unknown' : 
                   isPermissionGranted ? 'Granted' : 'Denied'}
                </Text>
              </div>
              
              <div>
                <Text strong>Recording Status: </Text>
                <Text type={isRecording ? 'warning' : 'secondary'}>
                  {isRecording ? 'Recording...' : 'Stopped'}
                </Text>
              </div>

              <div>
                <Text strong>Audio Level: </Text>
                <Progress 
                  percent={audioLevel} 
                  size="small" 
                  status={audioLevel > 50 ? 'active' : 'normal'}
                />
              </div>

              <Space>
                {!isRecording ? (
                  <Button 
                    type="primary" 
                    icon={<AudioOutlined />}
                    onClick={handleStartRecording}
                  >
                    Start Recording
                  </Button>
                ) : (
                  <Button 
                    type="primary" 
                    danger 
                    icon={<StopOutlined />}
                    onClick={handleStopRecording}
                  >
                    Stop Recording
                  </Button>
                )}
                
                {!isPermissionGranted && (
                  <Button onClick={requestPermission}>
                    Request Permission
                  </Button>
                )}
              </Space>

              {audioUrl && (
                <Space>
                  <Button 
                    icon={<PlayCircleOutlined />}
                    onClick={handlePlayRecording}
                  >
                    Play Recording
                  </Button>
                  <Button onClick={handleDownloadRecording}>
                    Download
                  </Button>
                </Space>
              )}
            </Space>
          </Card>
        </Col>

        {/* Noise Reduction Settings */}
        <Col span={12}>
          <Card title="Browser Noise Reduction Settings" bordered={false}>
            <Space direction="vertical" style={{ width: '100%' }}>
              <div>
                <Text strong>Enhanced Noise Reduction: </Text>
                <Switch 
                  checked={enhancedNoiseReduction}
                  onChange={setEnhancedNoiseReduction}
                  disabled={isRecording}
                />
              </div>

              <div>
                <Text strong>Current Setting: </Text>
                <Text type={isEnhancedNoiseReductionEnabled ? 'success' : 'secondary'}>
                  {isEnhancedNoiseReductionEnabled ? 'Enhanced (Google algorithms)' : 'Standard (Basic browser)'}
                </Text>
              </div>
              
              <Alert
                message="Browser Noise Reduction"
                description={
                  enhancedNoiseReduction 
                    ? "Using enhanced Google Chrome algorithms: echo cancellation, noise suppression, auto gain control, typing noise detection, and advanced filters."
                    : "Using standard browser noise reduction features only."
                }
                type="info"
                showIcon
                style={{ marginTop: '16px' }}
              />
            </Space>
          </Card>
        </Col>

        {/* Statistics */}
        <Col span={24}>
          <Card title="Statistics" bordered={false}>
            <Row gutter={16}>
              <Col span={6}>
                <Statistic 
                  title="Audio Level" 
                  value={audioLevel} 
                  suffix="%" 
                  valueStyle={{ color: audioLevel > 50 ? '#cf1322' : '#3f8600' }}
                />
              </Col>
              <Col span={6}>
                <Statistic 
                  title="Noise Reduction" 
                  value={isEnhancedNoiseReductionEnabled ? 'Enhanced' : 'Standard'} 
                  valueStyle={{ color: isEnhancedNoiseReductionEnabled ? '#3f8600' : '#d9d9d9' }}
                />
              </Col>
              <Col span={6}>
                <Statistic 
                  title="Permission" 
                  value={isPermissionGranted ? 'Granted' : 'Denied'} 
                  valueStyle={{ color: isPermissionGranted ? '#3f8600' : '#d9d9d9' }}
                />
              </Col>
              <Col span={6}>
                <Statistic 
                  title="Recording State" 
                  value={isRecording ? 'Active' : 'Inactive'} 
                  valueStyle={{ color: isRecording ? '#cf1322' : '#d9d9d9' }}
                />
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>

      {/* Instructions */}
      <Card title="Instructions" style={{ marginTop: '16px' }} bordered={false}>
        <div>
          <Text>
            <strong>How to test browser noise reduction:</strong>
          </Text>
          <ol>
            <li><strong>Enhanced Mode (Recommended):</strong> Uses Google Chrome's advanced algorithms including echo cancellation, noise suppression, auto gain control, typing noise detection, and high-pass filters.</li>
            <li><strong>Standard Mode:</strong> Uses basic browser noise reduction features only.</li>
            <li>Toggle the setting before starting recording (cannot change during recording).</li>
            <li>Test in different environments to hear the difference.</li>
            <li>Enhanced mode works best in Chrome browser.</li>
          </ol>
          
          <Alert
            message="Browser Compatibility"
            description="Enhanced noise reduction features work best in Google Chrome. Other browsers may have limited support for advanced audio processing constraints."
            type="warning"
            showIcon
            style={{ marginTop: '16px' }}
          />
        </div>
      </Card>
    </div>
  )
}

export default MicrophoneTestPage
