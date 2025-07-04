import React, { useState } from 'react'
import { Card, Row, Col, Button, Progress, Tag, List, Typography, Modal, Space } from 'antd'
import { 
  PlayCircleOutlined, 
  SoundOutlined, 
  EyeOutlined,
  CheckCircleOutlined,
  BookOutlined
} from '@ant-design/icons'

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

  const playPronunciation = (word: string) => {
    // Simulate pronunciation play
    console.log(`Playing pronunciation for: ${word}`)
  }

  return (
    <div>
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
                    icon={<SoundOutlined />}
                    onClick={() => playPronunciation(item.word)}
                  >
                    Phát âm
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
                  onClick={() => playPronunciation(selectedWord.word)}
                  style={{ marginLeft: '8px' }}
                />
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
    </div>
  )
}

export default VocabularySection
