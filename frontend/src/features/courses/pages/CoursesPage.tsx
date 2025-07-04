import { Typography, Row, Col, notification } from 'antd'
import CourseCard from '../components/CourseCard'

const { Title, Paragraph } = Typography

const CoursesPage: React.FC = () => {
  const sampleCourses = [
    {
      title: 'English for Beginners',
      description: 'Khóa học tiếng Anh cơ bản dành cho người mới bắt đầu',
      level: 'Beginner' as const,
      duration: '8 tuần',
      students: 150,
      instructor: 'Ms. Sarah Johnson',
      progress: 65
    },
    {
      title: 'Business English',
      description: 'Tiếng Anh thương mại cho môi trường công sở',
      level: 'Intermediate' as const,
      duration: '12 tuần',
      students: 89,
      instructor: 'Mr. David Smith'
    },
    {
      title: 'IELTS Preparation',
      description: 'Luyện thi IELTS toàn diện 4 kỹ năng',
      level: 'Advanced' as const,
      duration: '16 tuần',
      students: 75,
      instructor: 'Dr. Emily Chen'
    }
  ]

  return (
    <div>
      <Title level={2}>Quản lý Khóa học</Title>
      <Paragraph>
        Dưới đây là danh sách các khóa học hiện có trong hệ thống.
      </Paragraph>
      
      <Row gutter={[16, 16]}>
        {sampleCourses.map((course, index) => (
          <Col xs={24} lg={8} key={index}>
            <CourseCard
              {...course}
              onEnroll={() => {
                notification.success({
                  message: 'Đăng ký thành công!',
                  description: `Bạn đã đăng ký khóa học "${course.title}" thành công.`,
                })
              }}
            />
          </Col>
        ))}
      </Row>
    </div>
  )
}

export default CoursesPage
