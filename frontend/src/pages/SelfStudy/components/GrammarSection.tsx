import React, { useState } from 'react'
import { Card, Row, Col, Button, List, Typography, Progress, Space } from 'antd'
import { 
  PlayCircleOutlined, 
  CheckCircleOutlined,
  BookOutlined,
  ExperimentOutlined
} from '@ant-design/icons'

const { Title, Paragraph, Text } = Typography

interface GrammarTopic {
  id: number
  title: string
  description: string
  level: string
  progress: number
  lessons: number
  color: string
}

const GrammarSection: React.FC = () => {
  const [selectedLevel, setSelectedLevel] = useState('beginner')

  const grammarTopics: GrammarTopic[] = [
    {
      id: 1,
      title: 'Present Tenses',
      description: 'Các thì hiện tại: Present Simple, Present Continuous, Present Perfect',
      level: 'beginner',
      progress: 85,
      lessons: 12,
      color: '#52c41a'
    },
    {
      id: 2,
      title: 'Past Tenses',
      description: 'Các thì quá khứ: Past Simple, Past Continuous, Past Perfect',
      level: 'beginner',
      progress: 70,
      lessons: 10,
      color: '#1890ff'
    },
    {
      id: 3,
      title: 'Modal Verbs',
      description: 'Động từ khuyết thiếu: can, could, should, must, might',
      level: 'intermediate',
      progress: 60,
      lessons: 8,
      color: '#fa8c16'
    },
    {
      id: 4,
      title: 'Conditional Sentences',
      description: 'Câu điều kiện loại 1, 2, 3 và mixed conditionals',
      level: 'intermediate',
      progress: 45,
      lessons: 6,
      color: '#722ed1'
    },
    {
      id: 5,
      title: 'Passive Voice',
      description: 'Câu bị động trong các thì khác nhau',
      level: 'advanced',
      progress: 30,
      lessons: 5,
      color: '#eb2f96'
    }
  ]

  const levels = [
    { key: 'beginner', label: 'Cơ bản', color: '#52c41a' },
    { key: 'intermediate', label: 'Trung cấp', color: '#fa8c16' },
    { key: 'advanced', label: 'Nâng cao', color: '#f5222d' }
  ]

  const filteredTopics = grammarTopics.filter(topic => topic.level === selectedLevel)

  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>📖 Chọn cấp độ ngữ pháp</Title>
          <Row gutter={[12, 12]}>
            {levels.map(level => (
              <Col key={level.key} xs={8} sm={6} md={4}>
                <Button
                  type={selectedLevel === level.key ? 'primary' : 'default'}
                  style={{ 
                    width: '100%',
                    backgroundColor: selectedLevel === level.key ? level.color : undefined,
                    borderColor: level.color
                  }}
                  onClick={() => setSelectedLevel(level.key)}
                >
                  {level.label}
                </Button>
              </Col>
            ))}
          </Row>
        </Col>
      </Row>

      <Card 
        title={
          <Space>
            <ExperimentOutlined style={{ color: levels.find(l => l.key === selectedLevel)?.color }} />
            <span>Ngữ pháp {levels.find(l => l.key === selectedLevel)?.label}</span>
          </Space>
        }
      >
        <List
          grid={{ gutter: 16, xs: 1, sm: 2, md: 2, lg: 3, xl: 3, xxl: 3 }}
          dataSource={filteredTopics}
          renderItem={(topic) => (
            <List.Item>
              <Card
                hoverable
                actions={[
                  <Button type="primary" icon={<PlayCircleOutlined />} size="small">
                    Học ngay
                  </Button>,
                  <Button icon={<BookOutlined />} size="small">
                    Lý thuyết
                  </Button>
                ]}
              >
                <Card.Meta
                  title={
                    <Space>
                      <span>{topic.title}</span>
                      {topic.progress === 100 && (
                        <CheckCircleOutlined style={{ color: '#52c41a' }} />
                      )}
                    </Space>
                  }
                  description={
                    <div>
                      <Paragraph style={{ fontSize: '12px', marginBottom: '12px' }}>
                        {topic.description}
                      </Paragraph>
                      <div style={{ marginBottom: '8px' }}>
                        <Text style={{ fontSize: '12px' }}>
                          📚 {topic.lessons} bài học
                        </Text>
                      </div>
                      <Progress 
                        percent={topic.progress} 
                        size="small"
                        strokeColor={topic.color}
                      />
                    </div>
                  }
                />
              </Card>
            </List.Item>
          )}
        />
      </Card>
    </div>
  )
}

export default GrammarSection
