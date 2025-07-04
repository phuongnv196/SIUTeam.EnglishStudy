import React, { useState, useEffect } from 'react'
import {
  Modal,
  Form,
  Input,
  Button,
  Upload,
  Avatar,
  message,
  Typography,
  Space,
  Tabs,
  Row,
  Col,
} from 'antd'
import {
  UserOutlined,
  MailOutlined,
  CameraOutlined,
  LockOutlined,
  EyeInvisibleOutlined,
  EyeTwoTone,
} from '@ant-design/icons'
import type { UploadFile } from 'antd/es/upload/interface'
import { useAuth } from '../../contexts/AuthContext'
import AuthService from '../../services/authService'
import styles from './UserProfileModal.module.scss'

const { Text } = Typography
const { TabPane } = Tabs

interface UserProfileModalProps {
  open: boolean
  onClose: () => void
}

interface ProfileFormValues {
  firstName: string
  lastName: string
  email: string
  username: string
}

interface PasswordFormValues {
  currentPassword: string
  newPassword: string
  confirmPassword: string
}

const UserProfileModal: React.FC<UserProfileModalProps> = ({ open, onClose }) => {
  const [profileForm] = Form.useForm()
  const [passwordForm] = Form.useForm()
  const [loading, setLoading] = useState(false)
  const [passwordLoading, setPasswordLoading] = useState(false)
  const [avatar, setAvatar] = useState<string>('')
  const { user, updateUser, refreshUserData } = useAuth()

  // Initialize form values when modal opens
  useEffect(() => {
    if (user && open) {
      profileForm.setFieldsValue({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
        username: user.username,
      })
      setAvatar(user.avatar || '')
    }
  }, [user, open, profileForm])

  const handleProfileSubmit = async (values: ProfileFormValues) => {
    try {
      setLoading(true)
      const success = await updateUser(values)
      if (success) {
        await refreshUserData()
      }
    } finally {
      setLoading(false)
    }
  }

  const handlePasswordSubmit = async (values: PasswordFormValues) => {
    try {
      setPasswordLoading(true)
      await AuthService.changePassword(values.currentPassword, values.newPassword)
      message.success('Đổi mật khẩu thành công')
      passwordForm.resetFields()
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Đổi mật khẩu thất bại'
      message.error(errorMessage)
    } finally {
      setPasswordLoading(false)
    }
  }

  const handleAvatarChange = (info: any) => {
    if (info.file.status === 'done') {
      // Handle avatar upload success
      const avatarUrl = info.file.response?.url || ''
      setAvatar(avatarUrl)
      updateUser({ avatar: avatarUrl })
    }
  }

  const beforeUpload = (file: UploadFile) => {
    const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png'
    if (!isJpgOrPng) {
      message.error('Chỉ có thể tải lên file JPG/PNG!')
      return false
    }
    const isLt2M = (file.size || 0) / 1024 / 1024 < 2
    if (!isLt2M) {
      message.error('Kích thước ảnh phải nhỏ hơn 2MB!')
      return false
    }
    return true
  }

  const handleClose = () => {
    profileForm.resetFields()
    passwordForm.resetFields()
    onClose()
  }

  if (!user) return null

  return (
    <Modal
      title="Thông tin cá nhân"
      open={open}
      onCancel={handleClose}
      footer={null}
      width={600}
      centered
      destroyOnClose
    >
      <Tabs defaultActiveKey="profile" centered>
        <TabPane tab="Thông tin cơ bản" key="profile">
          <div style={{ padding: '20px 0' }}>
            {/* Avatar Section */}
            <div style={{ textAlign: 'center', marginBottom: '32px' }}>
              <Upload
                name="avatar"
                listType="picture-circle"
                className={styles.avatarUploader}
                showUploadList={false}
                action="/api/upload/avatar"
                beforeUpload={beforeUpload}
                onChange={handleAvatarChange}
                headers={{
                  Authorization: `Bearer ${AuthService.getToken()}`,
                }}
              >
                <Avatar
                  size={100}
                  src={avatar || user.avatar || undefined}
                  icon={<UserOutlined />}
                  style={{ cursor: 'pointer' }}
                />
                <div style={{ marginTop: '8px', fontSize: '12px', color: '#666' }}>
                  <CameraOutlined /> Thay đổi ảnh
                </div>
              </Upload>
            </div>

            {/* Profile Form */}
            <Form
              form={profileForm}
              layout="vertical"
              onFinish={handleProfileSubmit}
              requiredMark={false}
            >
              <Row gutter={16}>
                <Col span={12}>
                  <Form.Item
                    name="firstName"
                    label="Tên"
                    rules={[{ required: true, message: 'Vui lòng nhập tên!' }]}
                  >
                    <Input prefix={<UserOutlined />} size="large" />
                  </Form.Item>
                </Col>
                <Col span={12}>
                  <Form.Item
                    name="lastName"
                    label="Họ"
                    rules={[{ required: true, message: 'Vui lòng nhập họ!' }]}
                  >
                    <Input prefix={<UserOutlined />} size="large" />
                  </Form.Item>
                </Col>
              </Row>

              <Form.Item
                name="username"
                label="Tên đăng nhập"
                rules={[
                  { required: true, message: 'Vui lòng nhập tên đăng nhập!' },
                ]}
              >
                <Input prefix={<UserOutlined />} size="large" />
              </Form.Item>

              <Form.Item
                name="email"
                label="Email"
                rules={[
                  { required: true, message: 'Vui lòng nhập email!' },
                  { type: 'email', message: 'Email không hợp lệ!' },
                ]}
              >
                <Input prefix={<MailOutlined />} size="large" />
              </Form.Item>

              <Form.Item>
                <Space style={{ width: '100%', justifyContent: 'space-between' }}>
                  <div>
                    <Text type="secondary">Vai trò: </Text>
                    <Text strong>{user.role}</Text>
                  </div>
                  <div>
                    <Text type="secondary">Tham gia: </Text>
                    <Text>{new Date(user.createdAt).toLocaleDateString('vi-VN')}</Text>
                  </div>
                </Space>
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  size="large"
                  loading={loading}
                  block
                  style={{
                    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                    border: 'none',
                  }}
                >
                  Cập nhật thông tin
                </Button>
              </Form.Item>
            </Form>
          </div>
        </TabPane>

        <TabPane tab="Đổi mật khẩu" key="password">
          <div style={{ padding: '20px 0' }}>
            <Form
              form={passwordForm}
              layout="vertical"
              onFinish={handlePasswordSubmit}
              requiredMark={false}
            >
              <Form.Item
                name="currentPassword"
                label="Mật khẩu hiện tại"
                rules={[{ required: true, message: 'Vui lòng nhập mật khẩu hiện tại!' }]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Mật khẩu hiện tại"
                  size="large"
                  iconRender={(visible) =>
                    visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                  }
                />
              </Form.Item>

              <Form.Item
                name="newPassword"
                label="Mật khẩu mới"
                rules={[
                  { required: true, message: 'Vui lòng nhập mật khẩu mới!' },
                  { min: 6, message: 'Mật khẩu phải có ít nhất 6 ký tự!' },
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Mật khẩu mới"
                  size="large"
                  iconRender={(visible) =>
                    visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                  }
                />
              </Form.Item>

              <Form.Item
                name="confirmPassword"
                label="Xác nhận mật khẩu mới"
                dependencies={['newPassword']}
                rules={[
                  { required: true, message: 'Vui lòng xác nhận mật khẩu mới!' },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue('newPassword') === value) {
                        return Promise.resolve()
                      }
                      return Promise.reject(new Error('Mật khẩu xác nhận không khớp!'))
                    },
                  }),
                ]}
              >
                <Input.Password
                  prefix={<LockOutlined />}
                  placeholder="Xác nhận mật khẩu mới"
                  size="large"
                  iconRender={(visible) =>
                    visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                  }
                />
              </Form.Item>

              <Form.Item>
                <Button
                  type="primary"
                  htmlType="submit"
                  size="large"
                  loading={passwordLoading}
                  block
                  style={{
                    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                    border: 'none',
                  }}
                >
                  Đổi mật khẩu
                </Button>
              </Form.Item>
            </Form>
          </div>
        </TabPane>
      </Tabs>
    </Modal>
  )
}

export default UserProfileModal
