import React from 'react'
import { Typography, Row, Col, Card, Progress, List, Timeline, Tag, Statistic } from 'antd'
import { 
  TrophyOutlined, 
  FireOutlined,
  ClockCircleOutlined,
  RiseOutlined
} from '@ant-design/icons'
import styles from './ProgressPage.module.scss'

const { Title, Paragraph } = Typography

const ProgressPage: React.FC = () => {
  const learningProgress = [
    {
      subject: 'Grammar',
      progress: 85,
      level: 'B2',
      color: '#52c41a'
    },
    {
      subject: 'Vocabulary',
      progress: 72,
      level: 'B1',
      color: '#1890ff'
    },
    {
      subject: 'Speaking',
      progress: 68,
      level: 'B1',
      color: '#fa8c16'
    },
    {
      subject: 'Listening',
      progress: 90,
      level: 'B2',
      color: '#eb2f96'
    },
    {
      subject: 'Reading',
      progress: 78,
      level: 'B2',
      color: '#722ed1'
    },
    {
      subject: 'Writing',
      progress: 65,
      level: 'B1',
      color: '#13c2c2'
    }
  ]

  const achievements = [
    { title: 'Hoàn thành khóa học đầu tiên', date: '2024-12-15', type: 'course' },
    { title: 'Chuỗi học 30 ngày liên tiếp', date: '2024-12-10', type: 'streak' },
    { title: 'Đạt điểm 95% bài kiểm tra Grammar', date: '2024-12-05', type: 'test' },
    { title: 'Học 100 từ vựng mới', date: '2024-11-30', type: 'vocabulary' },
    { title: 'Hoàn thành Speaking Level B1', date: '2024-11-25', type: 'speaking' }
  ]

  const weeklyStats = [
    { day: 'T2', hours: 1.5 },
    { day: 'T3', hours: 2.0 },
    { day: 'T4', hours: 1.2 },
    { day: 'T5', hours: 2.5 },
    { day: 'T6', hours: 1.8 },
    { day: 'T7', hours: 3.0 },
    { day: 'CN', hours: 2.2 }
  ]

  return (
    <div className={styles.progressContainer}>
      <Title level={2}>📊 Tiến độ học tập của tôi</Title>
      <Paragraph>
        Theo dõi quá trình phát triển kỹ năng tiếng Anh của bạn qua thời gian.
      </Paragraph>

      {/* Overall Stats */}
      <Row gutter={[24, 24]} style={{ marginBottom: '24px' }}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Tổng giờ học"
              value={147}
              prefix={<ClockCircleOutlined />}
              suffix="giờ"
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Chuỗi học"
              value={15}
              prefix={<FireOutlined />}
              suffix="ngày"
              valueStyle={{ color: '#ff4d4f' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Thành tích"
              value={12}
              prefix={<TrophyOutlined />}
              suffix="huy hiệu"
              valueStyle={{ color: '#fa8c16' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Trình độ trung bình"
              value="B1+"
              prefix={<RiseOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={[24, 24]}>
        {/* Skills Progress */}
        <Col xs={24} lg={12}>
          <Card title="📚 Tiến độ theo kỹ năng" className={styles.progressCard}>
            <List
              dataSource={learningProgress}
              renderItem={(item) => (
                <List.Item>
                  <div style={{ width: '100%' }}>
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '8px' }}>
                      <span style={{ fontWeight: 500 }}>{item.subject}</span>
                      <Tag color={item.color}>{item.level}</Tag>
                    </div>
                    <Progress 
                      percent={item.progress} 
                      strokeColor={item.color}
                      size="small"
                    />
                  </div>
                </List.Item>
              )}
            />
          </Card>
        </Col>

        {/* Weekly Learning Hours */}
        <Col xs={24} lg={12}>
          <Card title="📈 Giờ học trong tuần" className={styles.statsCard}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'end', height: '200px', padding: '20px 0' }}>
              {weeklyStats.map((stat, index) => (
                <div key={index} style={{ textAlign: 'center', flex: 1 }}>
                  <div 
                    style={{ 
                      height: `${stat.hours * 30}px`,
                      backgroundColor: '#1890ff',
                      borderRadius: '4px 4px 0 0',
                      marginBottom: '8px',
                      opacity: stat.hours > 0 ? 1 : 0.3,
                      transition: 'all 0.3s ease'
                    }}
                  />
                  <div style={{ fontSize: '12px', fontWeight: 500 }}>{stat.day}</div>
                  <div style={{ fontSize: '10px', color: '#666' }}>{stat.hours}h</div>
                </div>
              ))}
            </div>
          </Card>
        </Col>

        {/* Recent Achievements */}
        <Col xs={24} lg={12}>
          <Card title="🏆 Thành tích gần đây" className={styles.achievementCard}>
            <Timeline>
              {achievements.map((achievement, index) => (
                <Timeline.Item 
                  key={index}
                  color={
                    achievement.type === 'course' ? 'green' :
                    achievement.type === 'streak' ? 'red' :
                    achievement.type === 'test' ? 'blue' :
                    achievement.type === 'vocabulary' ? 'orange' : 'purple'
                  }
                >
                  <p style={{ margin: 0, fontWeight: 500 }}>{achievement.title}</p>
                  <p style={{ margin: 0, fontSize: '12px', color: '#666' }}>{achievement.date}</p>
                </Timeline.Item>
              ))}
            </Timeline>
          </Card>
        </Col>

        {/* Learning Streak */}
        <Col xs={24} lg={12}>
          <Card title="🔥 Chuỗi học tập" className={styles.streakCard}>
            <div style={{ textAlign: 'center', padding: '20px 0' }}>
              <div style={{ fontSize: '48px', marginBottom: '16px' }}>🔥</div>
              <Title level={2} style={{ margin: 0, color: '#ff4d4f' }}>15 ngày</Title>
              <Paragraph style={{ margin: '8px 0', color: '#666' }}>
                Bạn đã học liên tiếp 15 ngày! Tuyệt vời!
              </Paragraph>
              <div style={{ display: 'flex', justifyContent: 'center', gap: '4px', marginTop: '16px' }}>
                {Array.from({ length: 7 }, (_, i) => (
                  <div
                    key={i}
                    style={{
                      width: '24px',
                      height: '24px',
                      borderRadius: '4px',
                      backgroundColor: i < 5 ? '#52c41a' : '#f0f0f0',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: '12px',
                      color: i < 5 ? 'white' : '#999'
                    }}
                  >
                    {i < 5 ? '✓' : ''}
                  </div>
                ))}
              </div>
              <Paragraph style={{ fontSize: '12px', color: '#999', marginTop: '8px' }}>
                Tuần này
              </Paragraph>
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  )
}

export default ProgressPage
