import React from 'react'
import { Alert, Space, Typography } from 'antd'

const { Text } = Typography

interface SpeakingTestResultsProps {
  lastTranscription: string
  pronunciationFeedback: {
    ipaNotation: string
    arpabet: string
  } | null
  onClose: () => void
}

const SpeakingTestResults: React.FC<SpeakingTestResultsProps> = ({
  lastTranscription,
  pronunciationFeedback,
  onClose
}) => {
  if (!lastTranscription) return null

  return (
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
      onClose={onClose}
    />
  )
}

export default SpeakingTestResults
