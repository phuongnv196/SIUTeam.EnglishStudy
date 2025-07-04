import React from 'react'
import { Card, Row, Col, Button, Typography, Progress, Space, Tag, Statistic, List } from 'antd'
import { 
  ThunderboltOutlined,
  PlayCircleOutlined,
  TrophyOutlined,
  ClockCircleOutlined,
  CheckCircleOutlined
} from '@ant-design/icons'

const { Title, Paragraph, Text } = Typography

interface CombinedExercise {
  id: number
  title: string
  description: string
  skills: string[]
  level: string
  duration: string
  points: number
  completed: boolean
  score?: number
}

const CombinedPracticeSection: React.FC = () => {

  const combinedExercises: CombinedExercise[] = [
    {
      id: 1,
      title: 'Business Presentation Challenge',
      description: 'Đọc tài liệu kinh doanh, nghe thuyết trình, viết tóm tắt và thực hành thuyết trình',
      skills: ['Reading', 'Listening', 'Writing', 'Speaking'],
      level: 'Advanced',
      duration: '45 min',
      points: 100,
      completed: true,
      score: 88
    },
    {
      id: 2,
      title: 'Travel Planning Project',
      description: 'Lên kế hoạch du lịch hoàn chỉnh bao gồm nghiên cứu, viết kế hoạch và thuyết trình',
      skills: ['Reading', 'Writing', 'Speaking', 'Vocabulary'],
      level: 'Intermediate',
      duration: '30 min',
      points: 75,
      completed: false
    },
    {
      id: 3,
      title: 'News Report Analysis',
      description: 'Đọc và nghe tin tức, phân tích nội dung, viết bài phân tích và thảo luận',
      skills: ['Reading', 'Listening', 'Writing', 'Grammar'],
      level: 'Advanced',
      duration: '50 min',
      points: 120,
      completed: false
    },
    {
      id: 4,
      title: 'Daily Life Scenarios',
      description: 'Kết hợp các tình huống hàng ngày với từ vựng, ngữ pháp và giao tiếp',
      skills: ['Vocabulary', 'Grammar', 'Conversation'],
      level: 'Beginner',
      duration: '20 min',
      points: 50,
      completed: true,
      score: 92
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

  const getSkillColor = (skill: string) => {
    const colors: { [key: string]: string } = {
      'Reading': '#722ed1',
      'Writing': '#eb2f96',
      'Listening': '#fa8c16',
      'Speaking': '#13c2c2',
      'Vocabulary': '#1890ff',
      'Grammar': '#52c41a',
      'Conversation': '#f5222d'
    }
    return colors[skill] || '#666'
  }

  const handleStartExercise = (exercise: CombinedExercise) => {
    // Simulate starting exercise
    console.log('Starting exercise:', exercise.title)
  }

  // Statistics
  const totalExercises = combinedExercises.length
  const completedExercises = combinedExercises.filter(ex => ex.completed).length
  const totalPoints = combinedExercises.filter(ex => ex.completed).reduce((sum, ex) => sum + ex.points, 0)
  const avgScore = combinedExercises.filter(ex => ex.completed && ex.score).reduce((sum, ex) => sum + (ex.score || 0), 0) / completedExercises || 0

  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>⚡ Luyện tập kết hợp</Title>
          <Paragraph>
            Thử thách bản thân với các bài tập tổng hợp nhiều kỹ năng, mô phỏng tình huống thực tế.
          </Paragraph>
        </Col>
      </Row>

      {/* Statistics Overview */}
      <Card style={{ marginBottom: '24px' }}>
        <Row gutter={[16, 16]}>
          <Col xs={12} sm={6}>
            <Statistic
              title="Hoàn thành"
              value={completedExercises}
              suffix={`/${totalExercises}`}
              prefix={<CheckCircleOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Col>
          <Col xs={12} sm={6}>
            <Statistic
              title="Tổng điểm"
              value={totalPoints}
              prefix={<TrophyOutlined />}
              valueStyle={{ color: '#fa8c16' }}
            />
          </Col>
          <Col xs={12} sm={6}>
            <Statistic
              title="Điểm TB"
              value={Math.round(avgScore)}
              suffix="/100"
              valueStyle={{ color: '#1890ff' }}
            />
          </Col>
          <Col xs={12} sm={6}>
            <Statistic
              title="Tiến độ"
              value={Math.round((completedExercises / totalExercises) * 100)}
              suffix="%"
              valueStyle={{ color: '#722ed1' }}
            />
          </Col>
        </Row>
      </Card>

      <Card 
        title={
          <Space>
            <ThunderboltOutlined style={{ color: '#f5222d' }} />
            <span>Bài tập tổng hợp</span>
          </Space>
        }
      >
        <List
          grid={{ gutter: 16, xs: 1, sm: 1, md: 2, lg: 2, xl: 2, xxl: 2 }}
          dataSource={combinedExercises}
          renderItem={(exercise) => (
            <List.Item>
              <Card
                hoverable
                actions={[
                  <Button 
                    type="primary" 
                    icon={<PlayCircleOutlined />}
                    onClick={() => handleStartExercise(exercise)}
                  >
                    {exercise.completed ? 'Làm lại' : 'Bắt đầu'}
                  </Button>
                ]}
              >
                <Card.Meta
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
                        style={{ marginBottom: '12px', fontSize: '13px' }}
                      >
                        {exercise.description}
                      </Paragraph>
                      
                      <div style={{ marginBottom: '12px' }}>
                        <Text strong style={{ fontSize: '12px' }}>Kỹ năng: </Text>
                        <Space wrap>
                          {exercise.skills.map(skill => (
                            <Tag 
                              key={skill} 
                              color={getSkillColor(skill)}
                              style={{ fontSize: '10px', padding: '2px 6px' }}
                            >
                              {skill}
                            </Tag>
                          ))}
                        </Space>
                      </div>
                      
                      <Space wrap style={{ marginBottom: '8px', fontSize: '12px' }}>
                        <Text>
                          <ClockCircleOutlined /> {exercise.duration}
                        </Text>
                        <Text>
                          <TrophyOutlined /> {exercise.points} điểm
                        </Text>
                        <Text style={{ color: getLevelColor(exercise.level) }}>
                          🎯 {exercise.level}
                        </Text>
                      </Space>
                      
                      {exercise.completed && exercise.score && (
                        <div style={{ marginTop: '8px' }}>
                          <Progress 
                            percent={exercise.score} 
                            size="small"
                            strokeColor={exercise.score >= 80 ? '#52c41a' : exercise.score >= 60 ? '#fa8c16' : '#f5222d'}
                          />
                          <Text style={{ fontSize: '12px', color: '#666' }}>
                            Điểm: {exercise.score}/100
                          </Text>
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

      {/* Difficulty Progression */}
      <Card title="📈 Lộ trình học tập" style={{ marginTop: '24px' }}>
        <Row gutter={[16, 16]}>
          <Col xs={24} sm={8}>
            <Card size="small" title="🟢 Cơ bản" style={{ borderColor: '#52c41a' }}>
              <Text>Bắt đầu với các kỹ năng cơ bản</Text>
              <Progress 
                percent={100} 
                size="small" 
                strokeColor="#52c41a"
                style={{ marginTop: '8px' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={8}>
            <Card size="small" title="🟡 Trung cấp" style={{ borderColor: '#fa8c16' }}>
              <Text>Kết hợp nhiều kỹ năng</Text>
              <Progress 
                percent={50} 
                size="small" 
                strokeColor="#fa8c16"
                style={{ marginTop: '8px' }}
              />
            </Card>
          </Col>
          <Col xs={24} sm={8}>
            <Card size="small" title="🔴 Nâng cao" style={{ borderColor: '#f5222d' }}>
              <Text>Thử thách toàn diện</Text>
              <Progress 
                percent={25} 
                size="small" 
                strokeColor="#f5222d"
                style={{ marginTop: '8px' }}
              />
            </Card>
          </Col>
        </Row>
      </Card>
    </div>
  )
}

export default CombinedPracticeSection
