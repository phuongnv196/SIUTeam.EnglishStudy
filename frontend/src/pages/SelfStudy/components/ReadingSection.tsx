import React from 'react'
import { Card, Row, Col, Button, List, Typography, Progress, Space, Avatar } from 'antd'
import { 
  ReadOutlined,
  EyeOutlined,
  ClockCircleOutlined
} from '@ant-design/icons'

const { Title, Paragraph, Text } = Typography

interface ReadingMaterial {
  id: number
  title: string
  excerpt: string
  readTime: string
  level: string
  category: string
  progress: number
  wordCount: number
}

const ReadingSection: React.FC = () => {
  const readingMaterials: ReadingMaterial[] = [
    {
      id: 1,
      title: 'The Future of Technology',
      excerpt: 'Artificial intelligence and machine learning are revolutionizing how we work and live...',
      readTime: '8 min',
      level: 'Advanced',
      category: 'Technology',
      progress: 100,
      wordCount: 650
    },
    {
      id: 2,
      title: 'Sustainable Tourism',
      excerpt: 'As travel becomes more accessible, the need for sustainable tourism practices grows...',
      readTime: '5 min',
      level: 'Intermediate',
      category: 'Environment',
      progress: 60,
      wordCount: 450
    },
    {
      id: 3,
      title: 'Daily Routines Around the World',
      excerpt: 'How people start their day varies greatly across different cultures and countries...',
      readTime: '3 min',
      level: 'Beginner',
      category: 'Culture',
      progress: 0,
      wordCount: 280
    },
    {
      id: 4,
      title: 'The Science of Sleep',
      excerpt: 'Recent research has revealed fascinating insights into why we sleep and how it affects our health...',
      readTime: '6 min',
      level: 'Intermediate',
      category: 'Health',
      progress: 25,
      wordCount: 520
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

  const getCategoryIcon = (category: string) => {
    switch (category) {
      case 'Technology': return '💻'
      case 'Environment': return '🌱'
      case 'Culture': return '🌍'
      case 'Health': return '🏥'
      default: return '📖'
    }
  }

  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>📚 Luyện đọc hiểu</Title>
          <Paragraph>
            Phát triển kỹ năng đọc hiểu thông qua các bài đọc đa dạng về nhiều chủ đề.
          </Paragraph>
        </Col>
      </Row>

      <Card 
        title={
          <Space>
            <ReadOutlined style={{ color: '#722ed1' }} />
            <span>Tài liệu đọc hiểu</span>
          </Space>
        }
      >
        <List
          grid={{ gutter: 16, xs: 1, sm: 1, md: 2, lg: 2, xl: 2, xxl: 2 }}
          dataSource={readingMaterials}
          renderItem={(material) => (
            <List.Item>
              <Card
                hoverable
                actions={[
                  <Button type="primary" icon={<EyeOutlined />}>
                    Đọc bài
                  </Button>,
                  <Button icon={<ReadOutlined />}>
                    Luyện tập
                  </Button>
                ]}
              >
                <Card.Meta
                  avatar={
                    <Avatar 
                      size={48}
                      style={{ 
                        backgroundColor: getLevelColor(material.level),
                        fontSize: '20px'
                      }}
                    >
                      {getCategoryIcon(material.category)}
                    </Avatar>
                  }
                  title={material.title}
                  description={
                    <div>
                      <Paragraph 
                        ellipsis={{ rows: 2 }}
                        style={{ marginBottom: '12px', fontSize: '13px' }}
                      >
                        {material.excerpt}
                      </Paragraph>
                      
                      <Space wrap style={{ marginBottom: '8px', fontSize: '12px' }}>
                        <Text>
                          <ClockCircleOutlined /> {material.readTime}
                        </Text>
                        <Text>📝 {material.wordCount} từ</Text>
                        <Text style={{ color: getLevelColor(material.level) }}>
                          🎯 {material.level}
                        </Text>
                        <Text>📂 {material.category}</Text>
                      </Space>
                      
                      <Progress 
                        percent={material.progress} 
                        size="small"
                        strokeColor={getLevelColor(material.level)}
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

export default ReadingSection
