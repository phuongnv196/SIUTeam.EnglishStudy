import React, { useState } from 'react'
import { Card, Row, Col, Button, List, Typography, Space, Avatar, Input, Rate } from 'antd'
import { 
  EditOutlined,
  SendOutlined,
  FileTextOutlined,
  CheckCircleOutlined
} from '@ant-design/icons'

const { Title, Paragraph, Text } = Typography
const { TextArea } = Input

interface WritingExercise {
  id: number
  title: string
  description: string
  type: string
  level: string
  timeLimit: string
  wordLimit: number
  completed: boolean
  score?: number
}

const WritingSection: React.FC = () => {
  const [selectedExercise, setSelectedExercise] = useState<WritingExercise | null>(null)
  const [userText, setUserText] = useState('')

  const writingExercises: WritingExercise[] = [
    {
      id: 1,
      title: 'Describe Your Hometown',
      description: 'Viết một đoạn văn mô tả quê hương của bạn, bao gồm địa điểm, con người và những điều đặc biệt.',
      type: 'Descriptive',
      level: 'Beginner',
      timeLimit: '20 min',
      wordLimit: 150,
      completed: true,
      score: 85
    },
    {
      id: 2,
      title: 'Opinion Essay: Remote Work',
      description: 'Viết một bài luận thể hiện quan điểm của bạn về làm việc từ xa và tác động của nó.',
      type: 'Opinion Essay',
      level: 'Intermediate',
      timeLimit: '45 min',
      wordLimit: 300,
      completed: false
    },
    {
      id: 3,
      title: 'Formal Email to Manager',
      description: 'Viết một email chính thức cho quản lý để xin nghỉ phép hoặc thảo luận về dự án.',
      type: 'Formal Email',
      level: 'Intermediate',
      timeLimit: '15 min',
      wordLimit: 200,
      completed: true,
      score: 92
    },
    {
      id: 4,
      title: 'Argumentative Essay: Technology',
      description: 'Viết một bài luận lập luận về tác động của công nghệ đối với giáo dục.',
      type: 'Argumentative',
      level: 'Advanced',
      timeLimit: '60 min',
      wordLimit: 500,
      completed: false
    }
  ]

  const getLevelColor = (level: string) => {
    switch (level) {
      case 'Beginner': return '#52c41a'
      case 'Intermediate': return '#fa8c16'
      case 'Advanced': return '#f5222d'
      default: return '#1890ff'
    }
  }

  const getTypeIcon = (type: string) => {
    switch (type) {
      case 'Descriptive': return '🖼️'
      case 'Opinion Essay': return '💭'
      case 'Formal Email': return '📧'
      case 'Argumentative': return '⚖️'
      default: return '✍️'
    }
  }

  const handleStartWriting = (exercise: WritingExercise) => {
    setSelectedExercise(exercise)
    setUserText('')
  }

  const handleSubmit = () => {
    // Simulate submission
    console.log('Submitted:', userText)
    setSelectedExercise(null)
    setUserText('')
  }

  if (selectedExercise) {
    return (
      <div>
        <Card 
          title={
            <Space>
              <span>{getTypeIcon(selectedExercise.type)}</span>
              <span>{selectedExercise.title}</span>
            </Space>
          }
          extra={
            <Button onClick={() => setSelectedExercise(null)}>
              ← Quay lại
            </Button>
          }
        >
          <Space direction="vertical" size="middle" style={{ width: '100%' }}>
            <div>
              <Paragraph strong>{selectedExercise.description}</Paragraph>
              <Space>
                <Text>⏱️ Thời gian: {selectedExercise.timeLimit}</Text>
                <Text>📝 Số từ: {selectedExercise.wordLimit} từ</Text>
                <Text style={{ color: getLevelColor(selectedExercise.level) }}>
                  🎯 {selectedExercise.level}
                </Text>
              </Space>
            </div>
            
            <TextArea
              placeholder="Bắt đầu viết bài của bạn..."
              value={userText}
              onChange={(e) => setUserText(e.target.value)}
              rows={12}
              showCount
              maxLength={selectedExercise.wordLimit * 7} // Approximate character limit
            />
            
            <div style={{ textAlign: 'right' }}>
              <Space>
                <Text>Số từ: {userText.split(' ').filter(word => word.length > 0).length}/{selectedExercise.wordLimit}</Text>
                <Button 
                  type="primary" 
                  icon={<SendOutlined />}
                  onClick={handleSubmit}
                  disabled={userText.trim().length === 0}
                >
                  Nộp bài
                </Button>
              </Space>
            </div>
          </Space>
        </Card>
      </div>
    )
  }

  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>✍️ Luyện kỹ năng viết</Title>
          <Paragraph>
            Phát triển kỹ năng viết tiếng Anh qua các bài tập đa dạng từ cơ bản đến nâng cao.
          </Paragraph>
        </Col>
      </Row>

      <Card 
        title={
          <Space>
            <FileTextOutlined style={{ color: '#eb2f96' }} />
            <span>Bài tập viết</span>
          </Space>
        }
      >
        <List
          grid={{ gutter: 16, xs: 1, sm: 1, md: 2, lg: 2, xl: 2, xxl: 2 }}
          dataSource={writingExercises}
          renderItem={(exercise) => (
            <List.Item>
              <Card
                hoverable
                actions={[
                  <Button 
                    type="primary" 
                    icon={<EditOutlined />}
                    onClick={() => handleStartWriting(exercise)}
                  >
                    {exercise.completed ? 'Làm lại' : 'Bắt đầu viết'}
                  </Button>
                ]}
              >
                <Card.Meta
                  avatar={
                    <Avatar 
                      size={48}
                      style={{ 
                        backgroundColor: getLevelColor(exercise.level),
                        fontSize: '20px'
                      }}
                    >
                      {getTypeIcon(exercise.type)}
                    </Avatar>
                  }
                  title={
                    <Space>
                      <span>{exercise.title}</span>
                      {exercise.completed && (
                        <CheckCircleOutlined style={{ color: '#52c41a' }} />
                      )}
                    </Space>
                  }
                  description={
                    <div>
                      <Paragraph 
                        ellipsis={{ rows: 2 }}
                        style={{ marginBottom: '12px', fontSize: '13px' }}
                      >
                        {exercise.description}
                      </Paragraph>
                      
                      <Space wrap style={{ marginBottom: '8px', fontSize: '12px' }}>
                        <Text>⏱️ {exercise.timeLimit}</Text>
                        <Text>📝 {exercise.wordLimit} từ</Text>
                        <Text style={{ color: getLevelColor(exercise.level) }}>
                          🎯 {exercise.level}
                        </Text>
                        <Text>📂 {exercise.type}</Text>
                      </Space>
                      
                      {exercise.completed && exercise.score && (
                        <div style={{ marginTop: '8px' }}>
                          <Text strong>Điểm số: </Text>
                          <Text style={{ color: '#52c41a' }}>{exercise.score}/100</Text>
                          <Rate 
                            disabled 
                            defaultValue={Math.round(exercise.score / 20)} 
                            style={{ marginLeft: '8px', fontSize: '12px' }}
                          />
                        </div>
                      )}
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

export default WritingSection
