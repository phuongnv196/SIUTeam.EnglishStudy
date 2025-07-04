import React, { useState, useEffect } from 'react'
import { Typography, Layout, Card, Row, Col, Progress, Statistic, Space, Tag } from 'antd'
import { useSearchParams } from 'react-router-dom'
import { 
  BookOutlined,
  BulbOutlined,
  SoundOutlined,
  ReadOutlined,
  EditOutlined,
  MessageOutlined,
  ThunderboltOutlined,
  TrophyOutlined,
  ClockCircleOutlined
} from '@ant-design/icons'

import styles from './SelfStudyPage.module.scss'
import VocabularySection from './components/VocabularySection'
import GrammarSection from './components/GrammarSection'
import ListeningSection from './components/ListeningSection'
import ReadingSection from './components/ReadingSection'
import WritingSection from './components/WritingSection'
import ConversationSection from './components/ConversationSection'
import CombinedPracticeSection from './components/CombinedPracticeSection'

const { Title, Paragraph } = Typography
const { Content } = Layout

interface SubMenuItem {
  key: string
  icon: React.ReactNode
  label: string
  description: string
  progress: number
  level: string
  color: string
}

const SelfStudyPage: React.FC = () => {
  const [searchParams] = useSearchParams()
  const [selectedSection, setSelectedSection] = useState('vocabulary')

  // Update selected section based on URL parameter
  useEffect(() => {
    const section = searchParams.get('section')
    if (section) {
      setSelectedSection(section)
    }
  }, [searchParams])

  const subMenuItems: SubMenuItem[] = [
    {
      key: 'vocabulary',
      icon: <BookOutlined />,
      label: 'Từ vựng',
      description: 'Học và ôn luyện từ vựng theo chủ đề',
      progress: 75,
      level: 'B2',
      color: '#1890ff'
    },
    {
      key: 'grammar',
      icon: <BulbOutlined />,
      label: 'Ngữ pháp',
      description: 'Nắm vững các quy tắc ngữ pháp cơ bản và nâng cao',
      progress: 85,
      level: 'B2',
      color: '#52c41a'
    },
    {
      key: 'listening',
      icon: <SoundOutlined />,
      label: 'Nghe nói',
      description: 'Luyện nghe và phát âm chuẩn',
      progress: 68,
      level: 'B1',
      color: '#fa8c16'
    },
    {
      key: 'reading',
      icon: <ReadOutlined />,
      label: 'Đọc',
      description: 'Phát triển kỹ năng đọc hiểu',
      progress: 78,
      level: 'B2',
      color: '#722ed1'
    },
    {
      key: 'writing',
      icon: <EditOutlined />,
      label: 'Viết',
      description: 'Luyện tập kỹ năng viết học thuật và giao tiếp',
      progress: 65,
      level: 'B1',
      color: '#eb2f96'
    },
    {
      key: 'conversation',
      icon: <MessageOutlined />,
      label: 'Hội thoại',
      description: 'Thực hành giao tiếp trong các tình huống thực tế',
      progress: 60,
      level: 'B1',
      color: '#13c2c2'
    },
    {
      key: 'combined',
      icon: <ThunderboltOutlined />,
      label: 'Luyện tập kết hợp',
      description: 'Kết hợp tất cả kỹ năng trong các bài tập tổng hợp',
      progress: 55,
      level: 'B1',
      color: '#f5222d'
    }
  ]

  const renderContent = () => {
    switch (selectedSection) {
      case 'vocabulary':
        return <VocabularySection />
      case 'grammar':
        return <GrammarSection />
      case 'listening':
        return <ListeningSection />
      case 'reading':
        return <ReadingSection />
      case 'writing':
        return <WritingSection />
      case 'conversation':
        return <ConversationSection />
      case 'combined':
        return <CombinedPracticeSection />
      default:
        return <VocabularySection />
    }
  }

  const currentSection = subMenuItems.find(item => item.key === selectedSection)

  return (
    <div className={styles.selfStudyContainer}>
      <div className={styles.header}>
        <Title level={2}>🎯 Tự học</Title>
        <Paragraph>
          Chọn kỹ năng bạn muốn luyện tập và cải thiện trình độ tiếng Anh của mình.
        </Paragraph>
      </div>

      {/* Overview Statistics */}
      <Card className={styles.overviewCard} style={{ marginBottom: '24px' }}>
        <Row gutter={[16, 16]}>
          <Col xs={24} sm={8} md={6}>
            <Statistic
              title="Tổng tiến độ"
              value={Math.round(subMenuItems.reduce((acc, item) => acc + item.progress, 0) / subMenuItems.length)}
              suffix="%"
              prefix={<TrophyOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Col>
          <Col xs={24} sm={8} md={6}>
            <Statistic
              title="Thời gian học hôm nay"
              value={45}
              suffix="phút"
              prefix={<ClockCircleOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Col>
          <Col xs={24} sm={8} md={6}>
            <Statistic
              title="Streak hiện tại"
              value={7}
              suffix="ngày"
              prefix={<ThunderboltOutlined />}
              valueStyle={{ color: '#fa8c16' }}
            />
          </Col>
          <Col xs={24} sm={8} md={6}>
            <Statistic
              title="Điểm cao nhất"
              value={95}
              suffix="/100"
              prefix={<TrophyOutlined />}
              valueStyle={{ color: '#eb2f96' }}
            />
          </Col>
        </Row>
      </Card>

      <Layout className={styles.studyLayout}>
        <Content className={styles.studyContent}>
          {currentSection && (
            <div className={styles.sectionHeader}>
              <Space size="middle" align="center">
                <div className={styles.sectionIcon} style={{ color: currentSection.color }}>
                  {currentSection.icon}
                </div>
                <div>
                  <Title level={3} style={{ margin: 0 }}>{currentSection.label}</Title>
                  <Paragraph style={{ margin: 0, color: '#666' }}>
                    {currentSection.description}
                  </Paragraph>
                </div>
                <Tag color={currentSection.color}>
                  {currentSection.level}
                </Tag>
              </Space>
              <Progress 
                percent={currentSection.progress} 
                strokeColor={currentSection.color}
                style={{ marginTop: '12px' }}
              />
            </div>
          )}
          
          <div className={styles.sectionContent}>
            {renderContent()}
          </div>
        </Content>
      </Layout>
    </div>
  )
}

export default SelfStudyPage
