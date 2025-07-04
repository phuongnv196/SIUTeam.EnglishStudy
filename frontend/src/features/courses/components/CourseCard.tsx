import { Card, Button, Progress, Tag, Space, Avatar } from 'antd'
import { BookOutlined, ClockCircleOutlined, UserOutlined } from '@ant-design/icons'

interface CourseCardProps {
  title: string
  description: string
  level: 'Beginner' | 'Intermediate' | 'Advanced'
  duration: string
  students: number
  progress?: number
  instructor: string
  onEnroll?: () => void
}

const CourseCard: React.FC<CourseCardProps> = ({
  title,
  description,
  level,
  duration,
  students,
  progress,
  instructor,
  onEnroll
}) => {
  const getLevelColor = (level: string) => {
    switch (level) {
      case 'Beginner': return 'green'
      case 'Intermediate': return 'orange'
      case 'Advanced': return 'red'
      default: return 'blue'
    }
  }

  return (
    <Card
      hoverable
      style={{ marginBottom: 16 }}
      actions={[
        <Button type="primary" onClick={onEnroll} block>
          {progress !== undefined ? 'Tiếp tục học' : 'Đăng ký khóa học'}
        </Button>
      ]}
    >
      <Card.Meta
        avatar={<Avatar icon={<BookOutlined />} size="large" />}
        title={
          <Space>
            {title}
            <Tag color={getLevelColor(level)}>{level}</Tag>
          </Space>
        }
        description={description}
      />
      
      <div style={{ marginTop: 16 }}>
        <Space direction="vertical" style={{ width: '100%' }}>
          {progress !== undefined && (
            <div>
              <div style={{ marginBottom: 8 }}>Tiến độ học tập:</div>
              <Progress percent={progress} />
            </div>
          )}
          
          <Space size="middle">
            <Space>
              <ClockCircleOutlined />
              <span>{duration}</span>
            </Space>
            <Space>
              <UserOutlined />
              <span>{students} học viên</span>
            </Space>
          </Space>
          
          <div>
            <strong>Giảng viên:</strong> {instructor}
          </div>
        </Space>
      </div>
    </Card>
  )
}

export default CourseCard
