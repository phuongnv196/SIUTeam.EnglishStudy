import { useState } from 'react'
import {
  Modal,
  Form,
  Input,
  Button,
  Typography,
  Divider,
  Space,
  Row,
  Col,
} from 'antd'
import {
  UserOutlined,
  MailOutlined,
  LockOutlined,
  EyeInvisibleOutlined,
  EyeTwoTone,
  UserAddOutlined,
} from '@ant-design/icons'
import { useAuth } from '../../contexts/AuthContext'

const { Title, Text, Link } = Typography

interface RegisterModalProps {
  open: boolean
  onClose: () => void
  onSwitchToLogin?: () => void
}

interface RegisterFormValues {
  username: string
  email: string
  password: string
  confirmPassword: string
  firstName: string
  lastName: string
}

const RegisterModal: React.FC<RegisterModalProps> = ({
  open,
  onClose,
  onSwitchToLogin,
}) => {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)
  const { register } = useAuth()

  const handleSubmit = async (values: RegisterFormValues) => {
    try {
      setLoading(true)
      const { confirmPassword, ...registerData } = values
      const success = await register(registerData)
      if (success) {
        form.resetFields()
        onClose()
        // Switch to login modal after successful registration
        if (onSwitchToLogin) {
          setTimeout(() => {
            onSwitchToLogin()
          }, 500)
        }
      }
    } finally {
      setLoading(false)
    }
  }

  const handleCancel = () => {
    form.resetFields()
    onClose()
  }

  return (
    <Modal
      title={null}
      open={open}
      onCancel={handleCancel}
      footer={null}
      width={500}
      centered
      destroyOnClose
    >
      <div style={{ padding: '20px 0' }}>
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          {/* Header */}
          <div style={{ textAlign: 'center' }}>
            <UserAddOutlined style={{ fontSize: '48px', color: '#667eea' }} />
            <Title level={3} style={{ margin: '16px 0 8px', color: '#667eea' }}>
              Đăng ký tài khoản
            </Title>
            <Text type="secondary">
              Tham gia cộng đồng học tiếng Anh English Study
            </Text>
          </div>

          {/* Register Form */}
          <Form
            form={form}
            name="register"
            onFinish={handleSubmit}
            layout="vertical"
            requiredMark={false}
          >
            <Row gutter={16}>
              <Col span={12}>
                <Form.Item
                  name="firstName"
                  label="Tên"
                  rules={[{ required: true, message: 'Vui lòng nhập tên!' }]}
                >
                  <Input
                    prefix={<UserOutlined />}
                    placeholder="Tên của bạn"
                    size="large"
                  />
                </Form.Item>
              </Col>
              <Col span={12}>
                <Form.Item
                  name="lastName"
                  label="Họ"
                  rules={[{ required: true, message: 'Vui lòng nhập họ!' }]}
                >
                  <Input
                    prefix={<UserOutlined />}
                    placeholder="Họ của bạn"
                    size="large"
                  />
                </Form.Item>
              </Col>
            </Row>

            <Form.Item
              name="username"
              label="Tên đăng nhập"
              rules={[
                { required: true, message: 'Vui lòng nhập tên đăng nhập!' },
                { min: 3, message: 'Tên đăng nhập phải có ít nhất 3 ký tự!' },
              ]}
            >
              <Input
                prefix={<UserOutlined />}
                placeholder="Tên đăng nhập"
                size="large"
              />
            </Form.Item>

            <Form.Item
              name="email"
              label="Email"
              rules={[
                { required: true, message: 'Vui lòng nhập email!' },
                { type: 'email', message: 'Email không hợp lệ!' },
              ]}
            >
              <Input
                prefix={<MailOutlined />}
                placeholder="Địa chỉ email"
                size="large"
              />
            </Form.Item>

            <Form.Item
              name="password"
              label="Mật khẩu"
              rules={[
                { required: true, message: 'Vui lòng nhập mật khẩu!' },
                { min: 6, message: 'Mật khẩu phải có ít nhất 6 ký tự!' },
              ]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                placeholder="Mật khẩu"
                size="large"
                iconRender={(visible) =>
                  visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                }
              />
            </Form.Item>

            <Form.Item
              name="confirmPassword"
              label="Xác nhận mật khẩu"
              dependencies={['password']}
              rules={[
                { required: true, message: 'Vui lòng xác nhận mật khẩu!' },
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    if (!value || getFieldValue('password') === value) {
                      return Promise.resolve()
                    }
                    return Promise.reject(new Error('Mật khẩu xác nhận không khớp!'))
                  },
                }),
              ]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                placeholder="Xác nhận mật khẩu"
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
                loading={loading}
                block
                style={{
                  background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                  border: 'none',
                  fontWeight: 'bold',
                }}
              >
                Đăng ký tài khoản
              </Button>
            </Form.Item>
          </Form>

          <Divider plain>
            <Text type="secondary">hoặc</Text>
          </Divider>

          {/* Switch to Login */}
          <div style={{ textAlign: 'center' }}>
            <Text type="secondary">
              Đã có tài khoản?{' '}
              <Link onClick={onSwitchToLogin} style={{ color: '#667eea' }}>
                Đăng nhập ngay
              </Link>
            </Text>
          </div>
        </Space>
      </div>
    </Modal>
  )
}

export default RegisterModal
