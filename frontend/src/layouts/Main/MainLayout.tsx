import { Layout, Menu, Button, Row, Typography, Avatar, Dropdown, Badge, Space } from 'antd'
import { 
  BookOutlined, 
  UserOutlined, 
  BellOutlined,
  SettingOutlined,
  LogoutOutlined,
  DashboardOutlined,
  BarChartOutlined,
  FileTextOutlined,
  ReadOutlined,
  BulbOutlined,
  SoundOutlined,
  EditOutlined,
  MessageOutlined,
  ThunderboltOutlined
} from '@ant-design/icons'
import type { ReactNode } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import { UserProfileModal } from '../../components/auth'
import { useState } from 'react'
import styles from './MainLayout.module.scss'

const { Header, Content, Footer, Sider } = Layout
const { Title } = Typography

interface MainLayoutProps {
  children: ReactNode
  selectedKey: string
  onMenuClick: (key: string) => void
  collapsed: boolean
  onCollapse: (collapsed: boolean) => void
  onNotificationClick: () => void
}

const MainLayout: React.FC<MainLayoutProps> = ({
  children,
  selectedKey,
  onMenuClick,
  collapsed,
  onCollapse,
  onNotificationClick
}) => {
  const [profileModalVisible, setProfileModalVisible] = useState(false)
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  // Handle logout with redirect to homepage
  const handleLogout = async () => {
    await logout(() => {
      navigate('/')
    })
  }

  const menuItems = [
    {
      key: '1',
      icon: <DashboardOutlined />,
      label: 'Trang chủ',
    },
    {
      key: '2',
      icon: <BookOutlined />,
      label: 'Khóa học',
    },
    {
      key: '3',
      icon: <BarChartOutlined />,
      label: 'Tiến độ của tôi',
    },
    {
      key: '4',
      icon: <FileTextOutlined />,
      label: 'Bài kiểm tra',
    },
    {
      key: '5',
      icon: <ReadOutlined />,
      label: 'Tự học',
      children: [
        {
          key: '5-1',
          icon: <BookOutlined />,
          label: 'Từ vựng',
        },
        {
          key: '5-2',
          icon: <BulbOutlined />,
          label: 'Ngữ pháp',
        },
        {
          key: '5-3',
          icon: <SoundOutlined />,
          label: 'Nghe nói',
        },
        {
          key: '5-4',
          icon: <ReadOutlined />,
          label: 'Đọc',
        },
        {
          key: '5-5',
          icon: <EditOutlined />,
          label: 'Viết',
        },
        {
          key: '5-6',
          icon: <MessageOutlined />,
          label: 'Hội thoại',
        },
        {
          key: '5-7',
          icon: <ThunderboltOutlined />,
          label: 'Luyện tập kết hợp',
        },
      ]
    },
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

  return (
    <Layout className={styles.mainLayout}>
      <Sider 
        collapsible 
        collapsed={collapsed} 
        onCollapse={onCollapse}
        theme="dark"
        className={`${styles.mainSidebar} ${collapsed ? styles.collapsed : ''}`}
        width={200}
        collapsedWidth={80}
      >
        <div style={{
          padding: '16px',
          textAlign: 'center',
          borderBottom: '1px solid rgba(255, 255, 255, 0.2)',
          marginBottom: '16px'
        }}>
          <Title level={4} style={{ 
            margin: 0, 
            color: 'white',
            fontWeight: 'bold'
          }}>
            <Link to="/" style={{ textDecoration: 'none', color: 'white' }}>{collapsed ? '📚' : '📚 English Study'}</Link>
          </Title>
        </div>
        <Menu
          theme="dark"
          selectedKeys={[selectedKey]}
          mode="inline"
          items={menuItems}
          onClick={({ key }) => onMenuClick(key)}
          className={styles.mainMenu}
        />
      </Sider>
      
      <Layout className={`${styles.mainContentLayout} ${collapsed ? styles.sidebarCollapsed : ''}`}>
        <Header className={styles.mainHeader}>
          <Row justify="space-between" align="middle" style={{ height: '100%' }}>
            <Title level={3} style={{ 
              margin: 0, 
              color: 'white',
              fontWeight: 'bold'
            }}>
              English Study - Học viên
            </Title>
            <Space size="middle">
              <Badge count={5} size="small">
                <Button 
                  type="text" 
                  icon={<BellOutlined />}
                  onClick={onNotificationClick}
                  style={{
                    color: 'white',
                    fontSize: '16px'
                  }}
                  className="notification-btn"
                >
                  Thông báo
                </Button>
              </Badge>
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
          </Row>
        </Header>
        
        <Content className={styles.mainContent} style={{ 
          margin: '24px 24px 0',
          padding: '24px',
          background: '#f8fafc',
          borderRadius: '12px 12px 0 0',
          minHeight: 'calc(100vh - 160px)',
          overflow: 'auto'
        }}>
          {children}
        </Content>
        
        <Footer className={styles.mainFooter} style={{ 
          textAlign: 'center',
          background: 'linear-gradient(90deg, #f8fafc 0%, #e2e8f0 100%)',
          color: '#64748b',
          padding: '16px',
          borderTop: '1px solid #e2e8f0'
        }}>
          <Space>
            <span>🎓 SIU Team English Study ©2025</span>
            <span>•</span>
            <span>Created with ❤️ using Ant Design</span>
            <span>•</span>
            <span>🚀 Empowering English Learning</span>
          </Space>
        </Footer>
      </Layout>

      {/* User Profile Modal */}
      <UserProfileModal
        open={profileModalVisible}
        onClose={() => setProfileModalVisible(false)}
      />
    </Layout>
  )
}

export default MainLayout
