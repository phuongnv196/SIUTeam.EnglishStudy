import { Space, Typography, Row, Col, Card, Statistic, Button, Progress, List, Tag } from 'antd'
import { 
  BookOutlined, 
  UserOutlined, 
  TrophyOutlined,
  RiseOutlined,
  FireOutlined,
  StarOutlined,
  CheckCircleOutlined
} from '@ant-design/icons'
import styles from './HomePage.module.scss'

const { Title, Paragraph } = Typography

interface HomePageProps {
  onNavigate: (key: string) => void
}

const HomePage: React.FC<HomePageProps> = ({ onNavigate }) => {
  const recentActivities = [
    { 
      title: 'Hoàn thành bài tập Grammar Unit 5', 
      time: '2 giờ trước',
      type: 'lesson',
      icon: <CheckCircleOutlined style={{ color: '#52c41a' }} />
    },
    { 
      title: 'Đạt điểm 95% trong bài kiểm tra Speaking', 
      time: '1 ngày trước',
      type: 'achievement',
      icon: <TrophyOutlined style={{ color: '#fa8c16' }} />
    },
    { 
      title: 'Bắt đầu khóa học Business English', 
      time: '2 ngày trước',
      type: 'course',
      icon: <BookOutlined style={{ color: '#667eea' }} />
    },
  ]

  const quickStats = [
    {
      title: 'Khóa học đã hoàn thành',
      value: 3,
      icon: <CheckCircleOutlined />,
      color: '#52c41a',
      suffix: 'khóa học'
    },
    {
      title: 'Bài tập đã làm',
      value: 127,
      icon: <BookOutlined />,
      color: '#1890ff',
      suffix: 'bài tập'
    },
    {
      title: 'Điểm trung bình',
      value: 4.6,
      icon: <StarOutlined />,
      color: '#fa8c16',
      suffix: '/ 5 điểm'
    },
    {
      title: 'Chuỗi học liên tiếp',
      value: 15,
      icon: <FireOutlined />,
      color: '#ff4d4f',
      suffix: 'ngày'
    }
  ]

  return (
    <div className={styles.dashboardContainer}>
      {/* Welcome Header */}
      <Card className={styles.welcomeCard} style={{
        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        color: 'white',
        border: 'none',
        marginBottom: '24px'
      }}>
        <Row align="middle" gutter={[24, 24]}>
          <Col flex="auto">
            <Title level={2} style={{ color: 'white', margin: 0 }}>
              🎉 Chào mừng bạn trở lại!
            </Title>
            <Paragraph style={{ color: 'rgba(255,255,255,0.9)', fontSize: '16px', margin: '8px 0 0' }}>
              Hôm nay là một ngày tuyệt vời để tiếp tục hành trình học tiếng Anh của bạn. Hãy cùng chinh phục những thử thách mới!
            </Paragraph>
          </Col>
          <Col>
            <div style={{ fontSize: '64px', opacity: 0.3 }}>📚</div>
          </Col>
        </Row>
      </Card>

      {/* Main Statistics */}
      <Row gutter={[24, 24]}>
        <Col xs={24} sm={12} lg={6}>
          <Card className={styles.statCard} hoverable>
            <Statistic
              title="Từ vựng đã học"
              value={856}
              prefix={<BookOutlined />}
              valueStyle={{ color: '#667eea', fontSize: '24px', fontWeight: 'bold' }}
            />
            <Tag color="green" style={{ marginTop: '8px' }}>+25 từ tuần này</Tag>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card className={styles.statCard} hoverable>
            <Statistic
              title="Giờ học đã hoàn thành"
              value={47}
              prefix={<UserOutlined />}
              valueStyle={{ color: '#52c41a', fontSize: '24px', fontWeight: 'bold' }}
            />
            <Progress percent={85} size="small" strokeColor="#52c41a" style={{ marginTop: '8px' }} />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card className={styles.statCard} hoverable>
            <Statistic
              title="Thành tích đạt được"
              value={12}
              prefix={<TrophyOutlined />}
              valueStyle={{ color: '#fa8c16', fontSize: '24px', fontWeight: 'bold' }}
            />
            <Tag color="orange" style={{ marginTop: '8px' }}>Xuất sắc</Tag>
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card className={styles.statCard} hoverable>
            <Statistic
              title="Trình độ hiện tại"
              value="B2"
              prefix={<RiseOutlined />}
              valueStyle={{ color: '#eb2f96', fontSize: '24px', fontWeight: 'bold' }}
            />
            <Tag color="purple" style={{ marginTop: '8px' }}>Trung cấp cao</Tag>
          </Card>
        </Col>
      </Row>

      {/* Quick Stats Row */}
      <Row gutter={[16, 16]} style={{ marginTop: '24px' }}>
        {quickStats.map((stat, index) => (
          <Col xs={12} sm={6} key={index}>
            <Card size="small" className={styles.quickStatCard}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                <div style={{ color: stat.color, fontSize: '20px' }}>
                  {stat.icon}
                </div>
                <div>
                  <div style={{ fontSize: '18px', fontWeight: 'bold', color: stat.color }}>
                    {stat.value}
                  </div>
                  <div style={{ fontSize: '12px', color: '#666' }}>
                    {stat.title}
                  </div>
                </div>
              </div>
            </Card>
          </Col>
        ))}
      </Row>

      {/* Content Grid */}
      <Row gutter={[24, 24]} style={{ marginTop: '24px' }}>
        {/* Recent Activities */}
        <Col xs={24} lg={12}>
          <Card title="🔥 Hoạt động gần đây" className={styles.activityCard}>
            <List
              dataSource={recentActivities}
              renderItem={(item) => (
                <List.Item>
                  <List.Item.Meta
                    avatar={item.icon}
                    title={item.title}
                    description={item.time}
                  />
                </List.Item>
              )}
            />
          </Card>
        </Col>

        {/* Quick Actions */}
        <Col xs={24} lg={12}>
          <Card title="⚡ Thao tác nhanh" className={styles.actionCard}>
            <Space direction="vertical" size="middle" style={{ width: '100%' }}>
              <Button 
                type="primary" 
                size="large" 
                block
                icon={<BookOutlined />}
                onClick={() => onNavigate('2')}
                style={{ height: '48px' }}
              >
                Quản lý khóa học
              </Button>
              <Button 
                size="large" 
                block
                icon={<UserOutlined />}
                onClick={() => onNavigate('3')}
                style={{ height: '48px' }}
              >
                Quản lý học viên
              </Button>
              <Button 
                size="large" 
                block
                icon={<RiseOutlined />}
                onClick={() => onNavigate('4')}
                style={{ height: '48px' }}
              >
                Xem thống kê
              </Button>
            </Space>
          </Card>
        </Col>
      </Row>
    </div>
  )
}

export default HomePage
