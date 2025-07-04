import { useState, createContext, useContext } from 'react'
import { Layout, Menu, Button, Row, Col, Typography, Drawer, Avatar, Dropdown, Space } from 'antd'
import { 
  HomeOutlined,
  BookOutlined,
  UserOutlined,
  MenuOutlined,
  LoginOutlined,
  UserAddOutlined,
  LogoutOutlined,
  SettingOutlined
} from '@ant-design/icons'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { LoginModal, RegisterModal, UserProfileModal } from '../../components/auth'
import styles from './LandingLayout.module.scss'

const { Header } = Layout
const { Title } = Typography

// Landing Context để share functions với LandingPage
interface LandingContextType {
  handleStartLearning: () => void
  openLoginModal: () => void
  openRegisterModal: () => void
}

const LandingContext = createContext<LandingContextType | null>(null)

export const useLandingContext = () => {
  const context = useContext(LandingContext)
  if (!context) {
    throw new Error('useLandingContext must be used within LandingLayout')
  }
  return context
}

interface LandingLayoutProps {
  children: React.ReactNode
}

const LandingLayout: React.FC<LandingLayoutProps> = ({ children }) => {
  const [mobileMenuVisible, setMobileMenuVisible] = useState(false)
  const [loginModalVisible, setLoginModalVisible] = useState(false)
  const [registerModalVisible, setRegisterModalVisible] = useState(false)
  const [profileModalVisible, setProfileModalVisible] = useState(false)
  
  const { user, isAuthenticated, logout } = useAuth()
  const navigate = useNavigate()

  // Function to handle "Bắt đầu học miễn phí" button
  const handleStartLearning = () => {
    if (isAuthenticated) {
      // Đã đăng nhập -> chuyển đến dashboard
      navigate('/dashboard')
    } else {
      // Chưa đăng nhập -> mở modal login
      setLoginModalVisible(true)
    }
  }

  // Additional helper functions
  const openLoginModal = () => setLoginModalVisible(true)
  const openRegisterModal = () => setRegisterModalVisible(true)

  // Handle logout with redirect to homepage
  const handleLogout = async () => {
    await logout(() => {
      navigate('/')
    })
  }

  // Landing context value
  const landingContextValue: LandingContextType = {
    handleStartLearning,
    openLoginModal,
    openRegisterModal
  }

  const menuItems = [
    {
      key: 'home',
      label: 'Trang chủ',
      icon: <HomeOutlined />
    },
    {
      key: 'courses',
      label: 'Khóa học',
      icon: <BookOutlined />
    },
    {
      key: 'about',
      label: 'Về chúng tôi',
      icon: <UserOutlined />
    }
  ]

  const userMenuItems = [
    {
      key: 'profile',
      icon: <UserOutlined />,
      label: 'Hồ sơ cá nhân',
      onClick: () => setProfileModalVisible(true),
    },
    {
      key: 'settings',
      icon: <SettingOutlined />,
      label: 'Cài đặt',
    },
    {
      type: 'divider' as const,
    },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Đăng xuất',
      onClick: handleLogout,
    },
  ]

  const scrollToSection = (sectionId: string) => {
    const element = document.getElementById(sectionId)
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' })
    }
    setMobileMenuVisible(false)
  }

  const handleSwitchToRegister = () => {
    setLoginModalVisible(false)
    setTimeout(() => {
      setRegisterModalVisible(true)
    }, 300)
  }

  const handleSwitchToLogin = () => {
    setRegisterModalVisible(false)
    setTimeout(() => {
      setLoginModalVisible(true)
    }, 300)
  }

  return (
    <LandingContext.Provider value={landingContextValue}>
      <Layout className={styles.landingLayout}>
        <Header className={styles.landingHeader}>
          <Row justify="space-between" align="middle" style={{ height: '100%' }}>
            <Col>
              <div className={styles.logo}>
                <Title level={3} style={{ margin: 0, color: 'white' }}>
                  English Study
                </Title>
              </div>
            </Col>
            
            {/* Desktop Auth Buttons */}
            <Col className={styles.desktopAuth}>
              {isAuthenticated ? (
                <Space>
                  <span style={{ color: 'white' }}>
                    Xin chào, {user?.firstName}
                  </span>
                  <Dropdown 
                    menu={{ items: userMenuItems }}
                    placement="bottomRight"
                    arrow
                  >
                    <Avatar 
                      style={{ 
                        backgroundColor: 'rgba(255, 255, 255, 0.2)',
                        border: '2px solid white',
                        cursor: 'pointer'
                      }}
                      src={user?.avatar || undefined}
                      icon={<UserOutlined />}
                    />
                  </Dropdown>
                </Space>
              ) : (
              <>
                <Button 
                  type="text" 
                  style={{ color: 'white', marginRight: 16 }}
                  icon={<LoginOutlined />}
                  onClick={() => setLoginModalVisible(true)}
                >
                  Đăng nhập
                </Button>
                <Button 
                  type="primary"
                  icon={<UserAddOutlined />}
                  style={{
                    background: 'linear-gradient(45deg, #ff6b6b, #ee5a52)',
                    border: 'none'
                  }}
                  onClick={() => setRegisterModalVisible(true)}
                >
                  Đăng ký
                </Button>
              </>
            )}
          </Col>
          
          {/* Mobile Menu Button */}
          <Col className={styles.mobileMenuButton}>
            <Button
              type="text"
              icon={<MenuOutlined />}
              onClick={() => setMobileMenuVisible(true)}
              style={{ color: 'white' }}
            />
          </Col>
        </Row>
      </Header>

      {/* Mobile Drawer Menu */}
      <Drawer
        title="English Study"
        placement="right"
        onClose={() => setMobileMenuVisible(false)}
        open={mobileMenuVisible}
        className={styles.mobileDrawer}
      >
        <Menu
          mode="inline"
          items={menuItems}
          onClick={({ key }) => scrollToSection(key)}
          style={{ border: 'none' }}
        />
        <div className={styles.mobileAuthButtons}>
          {isAuthenticated ? (
            <Space direction="vertical" style={{ width: '100%' }}>
              <Button 
                block 
                style={{ marginBottom: 16 }}
                icon={<UserOutlined />}
                onClick={() => {
                  setProfileModalVisible(true)
                  setMobileMenuVisible(false)
                }}
              >
                Hồ sơ cá nhân
              </Button>
              <Button 
                block
                icon={<LogoutOutlined />}
                onClick={() => {
                  handleLogout()
                  setMobileMenuVisible(false)
                }}
              >
                Đăng xuất
              </Button>
            </Space>
          ) : (
            <>
              <Button 
                block 
                style={{ marginBottom: 16 }}
                icon={<LoginOutlined />}
                onClick={() => {
                  setLoginModalVisible(true)
                  setMobileMenuVisible(false)
                }}
              >
                Đăng nhập
              </Button>
              <Button 
                type="primary" 
                block
                icon={<UserAddOutlined />}
                onClick={() => {
                  setRegisterModalVisible(true)
                  setMobileMenuVisible(false)
                }}
              >
                Đăng ký
              </Button>
            </>
          )}
        </div>
      </Drawer>

      <div className={styles.landingContent}>
        {children}
      </div>

      {/* Footer */}
      <footer className={styles.landingFooter}>
        <div className="container">
          <Row gutter={[32, 32]}>
            <Col xs={24} md={8}>
              <Title level={4} style={{ color: 'white' }}>
                English Study
              </Title>
              <p style={{ color: 'rgba(255,255,255,0.8)' }}>
                Nền tảng học tiếng Anh trực tuyến hàng đầu Việt Nam. 
                Chúng tôi cam kết mang đến trải nghiệm học tập tốt nhất.
              </p>
            </Col>
            <Col xs={24} md={8}>
              <Title level={5} style={{ color: 'white' }}>
                Liên kết nhanh
              </Title>
              <ul className={styles.footerLinks}>
                <li><a href="#home">Trang chủ</a></li>
                <li><a href="#courses">Khóa học</a></li>
                <li><a href="#about">Về chúng tôi</a></li>
              </ul>
            </Col>
          </Row>
          <div className={styles.footerBottom}>
            <p>&copy; 2025 English Study Platform. All rights reserved.</p>
          </div>
        </div>
      </footer>

      {/* Auth Modals */}
      <LoginModal
        open={loginModalVisible}
        onClose={() => setLoginModalVisible(false)}
        onSwitchToRegister={handleSwitchToRegister}
      />
      
      <RegisterModal
        open={registerModalVisible}
        onClose={() => setRegisterModalVisible(false)}
        onSwitchToLogin={handleSwitchToLogin}
      />
      
      <UserProfileModal
        open={profileModalVisible}
        onClose={() => setProfileModalVisible(false)}
      />
      </Layout>
    </LandingContext.Provider>
  )
}

export default LandingLayout
