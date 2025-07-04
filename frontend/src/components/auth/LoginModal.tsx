import { useState } from 'react'
import {
  Modal,
  Form,
  Input,
  Button,
  Checkbox,
  Typography,
  Divider,
  Space,
  Row,
  Col,
} from 'antd'
import {
  UserOutlined,
  LockOutlined,
  EyeInvisibleOutlined,
  EyeTwoTone,
  LoginOutlined,
} from '@ant-design/icons'
import { useAuth } from '../../contexts/AuthContext'

const { Title, Text, Link } = Typography

interface LoginModalProps {
  open: boolean
  onClose: () => void
  onSwitchToRegister: () => void
}

interface LoginFormValues {
  email: string
  password: string
  remember: boolean
}

const LoginModal: React.FC<LoginModalProps> = ({
  open,
  onClose,
  onSwitchToRegister,
}) => {
  const [form] = Form.useForm()
  const [loading, setLoading] = useState(false)
  const { login } = useAuth()

  const handleSubmit = async (values: LoginFormValues) => {
    try {
      setLoading(true)
      const success = await login(values.email, values.password)
      if (success) {
        form.resetFields()
        onClose()
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
      width={400}
      centered
      destroyOnClose
    >
      <div style={{ padding: '20px 0' }}>
        <Space direction="vertical" size="large" style={{ width: '100%' }}>
          {/* Header */}
          <div style={{ textAlign: 'center' }}>
            <LoginOutlined style={{ fontSize: '48px', color: '#667eea' }} />
            <Title level={3} style={{ margin: '16px 0 8px', color: '#667eea' }}>
              Đăng nhập
            </Title>
            <Text type="secondary">
              Chào mừng bạn trở lại với English Study!
            </Text>
          </div>

          {/* Login Form */}
          <Form
            form={form}
            name="login"
            onFinish={handleSubmit}
            layout="vertical"
            requiredMark={false}
          >
            <Form.Item
              name="email"
              label="Email"
              rules={[
                { required: true, message: 'Vui lòng nhập email!' },
                { type: 'email', message: 'Email không hợp lệ!' },
              ]}
            >
              <Input
                prefix={<UserOutlined />}
                placeholder="Nhập email của bạn"
                size="large"
              />
            </Form.Item>

            <Form.Item
              name="password"
              label="Mật khẩu"
              rules={[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]}
            >
              <Input.Password
                prefix={<LockOutlined />}
                placeholder="Nhập mật khẩu"
                size="large"
                iconRender={(visible) =>
                  visible ? <EyeTwoTone /> : <EyeInvisibleOutlined />
                }
              />
            </Form.Item>

            <Form.Item>
              <Row justify="space-between" align="middle">
                <Col>
                  <Form.Item name="remember" valuePropName="checked" noStyle>
                    <Checkbox>Ghi nhớ đăng nhập</Checkbox>
                  </Form.Item>
                </Col>
                <Col>
                  <Link href="#" style={{ color: '#667eea' }}>
                    Quên mật khẩu?
                  </Link>
                </Col>
              </Row>
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
                Đăng nhập
              </Button>
            </Form.Item>
          </Form>

          <Divider plain>
            <Text type="secondary">hoặc</Text>
          </Divider>

          {/* Switch to Register */}
          <div style={{ textAlign: 'center' }}>
            <Text type="secondary">
              Chưa có tài khoản?{' '}
              <Link onClick={onSwitchToRegister} style={{ color: '#667eea' }}>
                Đăng ký ngay
              </Link>
            </Text>
          </div>
        </Space>
      </div>
    </Modal>
  )
}

export default LoginModal
