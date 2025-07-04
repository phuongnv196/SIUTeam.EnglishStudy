import React, { useState } from 'react'
import { Card, Row, Col, Button, List, Typography, Progress, Space, Avatar } from 'antd'
import { 
  PlayCircleOutlined, 
  PauseCircleOutlined,
  SoundOutlined,
  AudioOutlined
} from '@ant-design/icons'

const { Title, Paragraph, Text } = Typography

interface ListeningLesson {
  id: number
  title: string
  description: string
  duration: string
  level: string
  type: string
  progress: number
  thumbnail: string
}

const ListeningSection: React.FC = () => {
  const [playingId, setPlayingId] = useState<number | null>(null)

  const listeningLessons: ListeningLesson[] = [
    {
      id: 1,
      title: 'Business Meeting Conversation',
      description: 'Học cách giao tiếp trong cuộc họp công việc',
      duration: '5:30',
      level: 'Intermediate',
      type: 'Conversation',
      progress: 75,
      thumbnail: '🏢'
    },
    {
      id: 2,
      title: 'Airport Announcements',
      description: 'Hiểu các thông báo tại sân bay',
      duration: '3:45',
      level: 'Beginner',
      type: 'Announcement',
      progress: 100,
      thumbnail: '✈️'
    },
    {
      id: 3,
      title: 'News Broadcast',
      description: 'Luyện nghe tin tức tiếng Anh',
      duration: '8:20',
      level: 'Advanced',
      type: 'News',
      progress: 30,
      thumbnail: '📰'
    },
    {
      id: 4,
      title: 'Daily Life Conversations',
      description: 'Hội thoại trong cuộc sống hàng ngày',
      duration: '6:15',
      level: 'Beginner',
      type: 'Daily',
      progress: 50,
      thumbnail: '🏠'
    }
  ]

  const handlePlayPause = (id: number) => {
    if (playingId === id) {
      setPlayingId(null)
    } else {
      setPlayingId(id)
    }
  }

  const getLevelColor = (level: string) => {
    switch (level) {
      case 'Beginner': return '#52c41a'
      case 'Intermediate': return '#fa8c16'
      case 'Advanced': return '#f5222d'
      default: return '#1890ff'
    }
  }

  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>🎧 Luyện nghe và phát âm</Title>
          <Paragraph>
            Cải thiện kỹ năng nghe hiểu và phát âm thông qua các bài học đa dạng.
          </Paragraph>
        </Col>
      </Row>

      <Card 
        title={
          <Space>
            <AudioOutlined style={{ color: '#1890ff' }} />
            <span>Bài học luyện nghe</span>
          </Space>
        }
      >
        <List
          dataSource={listeningLessons}
          renderItem={(lesson) => (
            <List.Item
              actions={[
                <Button 
                  type="primary"
                  icon={playingId === lesson.id ? <PauseCircleOutlined /> : <PlayCircleOutlined />}
                  onClick={() => handlePlayPause(lesson.id)}
                >
                  {playingId === lesson.id ? 'Tạm dừng' : 'Phát'}
                </Button>,
                <Button icon={<SoundOutlined />}>
                  Luyện phát âm
                </Button>
              ]}
            >
              <List.Item.Meta
                avatar={
                  <Avatar 
                    size={64} 
                    style={{ 
                      backgroundColor: getLevelColor(lesson.level),
                      fontSize: '24px'
                    }}
                  >
                    {lesson.thumbnail}
                  </Avatar>
                }
                title={
                  <Space>
                    <span>{lesson.title}</span>
                    <Text type="secondary">({lesson.duration})</Text>
                  </Space>
                }
                description={
                  <div>
                    <Paragraph style={{ marginBottom: '8px' }}>
                      {lesson.description}
                    </Paragraph>
                    <Space>
                      <Text strong>Cấp độ:</Text>
                      <Text style={{ color: getLevelColor(lesson.level) }}>
                        {lesson.level}
                      </Text>
                      <Text strong>Loại:</Text>
                      <Text>{lesson.type}</Text>
                    </Space>
                    <Progress 
                      percent={lesson.progress} 
                      size="small"
                      strokeColor={getLevelColor(lesson.level)}
                      style={{ marginTop: '8px' }}
                    />
                  </div>
                }
              />
            </List.Item>
          )}
        />
      </Card>
    </div>
  )
}

export default ListeningSection
