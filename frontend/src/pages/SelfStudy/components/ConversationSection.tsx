import React, { useState } from 'react'
import { Card, Row, Col, Button, List, Typography, Space, Avatar, Modal } from 'antd'
import { 
  MessageOutlined,
  PlayCircleOutlined,
  SoundOutlined,
  UserOutlined
} from '@ant-design/icons'

const { Title, Paragraph, Text } = Typography

interface ConversationScenario {
  id: number
  title: string
  description: string
  situation: string
  level: string
  participants: string[]
  duration: string
  dialogue: ConversationLine[]
}

interface ConversationLine {
  speaker: string
  text: string
  translation?: string
}

const ConversationSection: React.FC = () => {
  const [selectedScenario, setSelectedScenario] = useState<ConversationScenario | null>(null)
  const [modalVisible, setModalVisible] = useState(false)
  const [currentLine, setCurrentLine] = useState(0)

  const conversationScenarios: ConversationScenario[] = [
    {
      id: 1,
      title: 'At the Restaurant',
      description: 'Học cách đặt món và giao tiếp tại nhà hàng',
      situation: 'Bạn đang ở nhà hàng và muốn đặt món ăn',
      level: 'Beginner',
      participants: ['Customer', 'Waiter'],
      duration: '3 min',
      dialogue: [
        { speaker: 'Waiter', text: 'Good evening! Welcome to our restaurant. How many people?', translation: 'Chào buổi tối! Chào mừng đến nhà hàng. Mấy người ạ?' },
        { speaker: 'Customer', text: 'Good evening. Table for two, please.', translation: 'Chào buổi tối. Bàn cho hai người.' },
        { speaker: 'Waiter', text: 'Right this way, please. Here are your menus.', translation: 'Mời đi theo tôi. Đây là thực đơn.' },
        { speaker: 'Customer', text: 'Thank you. Could you recommend something?', translation: 'Cảm ơn. Bạn có thể gợi ý món nào không?' }
      ]
    },
    {
      id: 2,
      title: 'Job Interview',
      description: 'Thực hành phỏng vấn xin việc bằng tiếng Anh',
      situation: 'Bạn đang tham gia buổi phỏng vấn cho vị trí mong muốn',
      level: 'Intermediate',
      participants: ['Interviewer', 'Candidate'],
      duration: '5 min',
      dialogue: [
        { speaker: 'Interviewer', text: 'Please have a seat. Tell me about yourself.', translation: 'Mời ngồi. Hãy kể về bản thân bạn.' },
        { speaker: 'Candidate', text: 'Thank you. I have 3 years of experience in marketing...', translation: 'Cảm ơn. Tôi có 3 năm kinh nghiệm trong marketing...' },
        { speaker: 'Interviewer', text: 'What are your strengths and weaknesses?', translation: 'Điểm mạnh và điểm yếu của bạn là gì?' },
        { speaker: 'Candidate', text: 'My strength is attention to detail, and I\'m working on time management.', translation: 'Điểm mạnh là chú ý đến chi tiết, tôi đang cải thiện quản lý thời gian.' }
      ]
    },
    {
      id: 3,
      title: 'At the Airport',
      description: 'Giao tiếp tại sân bay khi check-in và qua cửa khẩu',
      situation: 'Bạn đang ở sân bay để check-in chuyến bay',
      level: 'Beginner',
      participants: ['Passenger', 'Check-in Staff'],
      duration: '4 min',
      dialogue: [
        { speaker: 'Staff', text: 'Good morning. May I see your passport and ticket?', translation: 'Chào buổi sáng. Cho tôi xem hộ chiếu và vé máy bay?' },
        { speaker: 'Passenger', text: 'Here you are. I\'d like a window seat if possible.', translation: 'Đây ạ. Tôi muốn chỗ ngồi cạnh cửa sổ nếu được.' },
        { speaker: 'Staff', text: 'Let me check... Yes, I can give you seat 15A.', translation: 'Để tôi kiểm tra... Được, tôi có thể cho bạn ghế 15A.' },
        { speaker: 'Passenger', text: 'Perfect! How many bags can I check in?', translation: 'Tuyệt! Tôi có thể ký gửi bao nhiêu túi?' }
      ]
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

  const getSituationIcon = (title: string) => {
    if (title.includes('Restaurant')) return '🍽️'
    if (title.includes('Interview')) return '💼'
    if (title.includes('Airport')) return '✈️'
    if (title.includes('Shopping')) return '🛒'
    return '💬'
  }

  const handlePractice = (scenario: ConversationScenario) => {
    setSelectedScenario(scenario)
    setCurrentLine(0)
    setModalVisible(true)
  }

  const nextLine = () => {
    if (selectedScenario && currentLine < selectedScenario.dialogue.length - 1) {
      setCurrentLine(currentLine + 1)
    }
  }

  const prevLine = () => {
    if (currentLine > 0) {
      setCurrentLine(currentLine - 1)
    }
  }

  return (
    <div>
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        <Col span={24}>
          <Title level={4}>💬 Luyện hội thoại</Title>
          <Paragraph>
            Thực hành giao tiếp trong các tình huống thực tế hàng ngày.
          </Paragraph>
        </Col>
      </Row>

      <Card 
        title={
          <Space>
            <MessageOutlined style={{ color: '#13c2c2' }} />
            <span>Tình huống hội thoại</span>
          </Space>
        }
      >
        <List
          grid={{ gutter: 16, xs: 1, sm: 1, md: 2, lg: 2, xl: 2, xxl: 2 }}
          dataSource={conversationScenarios}
          renderItem={(scenario) => (
            <List.Item>
              <Card
                hoverable
                actions={[
                  <Button 
                    type="primary" 
                    icon={<PlayCircleOutlined />}
                    onClick={() => handlePractice(scenario)}
                  >
                    Thực hành
                  </Button>,
                  <Button icon={<SoundOutlined />}>
                    Nghe mẫu
                  </Button>
                ]}
              >
                <Card.Meta
                  avatar={
                    <Avatar 
                      size={48}
                      style={{ 
                        backgroundColor: getLevelColor(scenario.level),
                        fontSize: '20px'
                      }}
                    >
                      {getSituationIcon(scenario.title)}
                    </Avatar>
                  }
                  title={scenario.title}
                  description={
                    <div>
                      <Paragraph 
                        style={{ marginBottom: '8px', fontSize: '13px' }}
                      >
                        {scenario.description}
                      </Paragraph>
                      
                      <Paragraph 
                        style={{ marginBottom: '12px', fontSize: '12px', fontStyle: 'italic', color: '#666' }}
                      >
                        💭 {scenario.situation}
                      </Paragraph>
                      
                      <Space wrap style={{ fontSize: '12px' }}>
                        <Text>⏱️ {scenario.duration}</Text>
                        <Text>👥 {scenario.participants.join(', ')}</Text>
                        <Text style={{ color: getLevelColor(scenario.level) }}>
                          🎯 {scenario.level}
                        </Text>
                      </Space>
                    </div>
                  }
                />
              </Card>
            </List.Item>
          )}
        />
      </Card>

      <Modal
        title={selectedScenario?.title}
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        width={800}
        footer={[
          <Button key="prev" onClick={prevLine} disabled={currentLine === 0}>
            ← Câu trước
          </Button>,
          <Button 
            key="next" 
            type="primary" 
            onClick={nextLine}
            disabled={!selectedScenario || currentLine >= selectedScenario.dialogue.length - 1}
          >
            Câu tiếp theo →
          </Button>,
          <Button key="close" onClick={() => setModalVisible(false)}>
            Đóng
          </Button>
        ]}
      >
        {selectedScenario && (
          <div>
            <div style={{ marginBottom: '16px', padding: '12px', backgroundColor: '#f6f8fa', borderRadius: '6px' }}>
              <Text strong>Tình huống: </Text>
              <Text>{selectedScenario.situation}</Text>
            </div>
            
            <div style={{ marginBottom: '16px' }}>
              <Text strong>Câu {currentLine + 1}/{selectedScenario.dialogue.length}</Text>
            </div>

            <Card>
              <Space direction="vertical" size="middle" style={{ width: '100%' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                  <Avatar icon={<UserOutlined />} />
                  <Text strong>{selectedScenario.dialogue[currentLine].speaker}:</Text>
                </div>
                
                <div style={{ padding: '16px', backgroundColor: '#f0f2f5', borderRadius: '8px' }}>
                  <Text style={{ fontSize: '16px', lineHeight: '1.6' }}>
                    {selectedScenario.dialogue[currentLine].text}
                  </Text>
                </div>
                
                {selectedScenario.dialogue[currentLine].translation && (
                  <div style={{ padding: '12px', backgroundColor: '#e6f7ff', borderRadius: '8px' }}>
                    <Text style={{ fontSize: '14px', color: '#666' }}>
                      💭 {selectedScenario.dialogue[currentLine].translation}
                    </Text>
                  </div>
                )}
                
                <div style={{ textAlign: 'center' }}>
                  <Button type="primary" icon={<SoundOutlined />}>
                    Phát âm
                  </Button>
                </div>
              </Space>
            </Card>
          </div>
        )}
      </Modal>
    </div>
  )
}

export default ConversationSection
