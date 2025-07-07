import React from 'react'
import { Typography, Row, Col, Card, Button, Progress } from 'antd'
import { PlayCircleOutlined } from '@ant-design/icons'

const { Title, Text } = Typography

interface Topic {
  key: string
  label: string
  color: string
  progress: number
}

interface TopicSelectorProps {
  topics: Topic[]
  selectedTopic: string
  onTopicSelect: (topicKey: string) => void
  onStartFlashcardStudy: (topicKey: string) => void
}

const TopicSelector: React.FC<TopicSelectorProps> = ({
  topics,
  selectedTopic,
  onTopicSelect,
  onStartFlashcardStudy
}) => {
  return (
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
                onClick={() => onTopicSelect(topic.key)}
                actions={[
                  <Button 
                    key="study"
                    size="small" 
                    type="text"
                    icon={<PlayCircleOutlined />}
                    onClick={(e) => {
                      e.stopPropagation()
                      onTopicSelect(topic.key)
                      setTimeout(() => onStartFlashcardStudy(topic.key), 100)
                    }}
                  >
                    Học
                  </Button>
                ]}
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
  )
}

export default TopicSelector
