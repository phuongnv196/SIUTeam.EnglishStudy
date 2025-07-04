import type { ReactNode } from 'react'
import { Result, Button, Spin } from 'antd'
import { 
  LockOutlined, 
  UserOutlined
} from '@ant-design/icons'
import { useAuth } from '../../contexts/AuthContext'

interface ProtectedRouteProps {
  children: ReactNode
  requiredRole?: string | string[]
  fallback?: ReactNode
  requireAuth?: boolean
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  requiredRole,
  fallback,
  requireAuth = true,
}) => {
  const { user, isLoading, isAuthenticated, hasRole } = useAuth()

  // Show loading spinner while checking authentication
  if (isLoading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        minHeight: '400px' 
      }}>
        <Spin size="large" tip="Đang kiểm tra quyền truy cập..." />
      </div>
    )
  }

  // Check if authentication is required
  if (requireAuth && !isAuthenticated) {
    return fallback || (
      <Result
        status="403"
        title="Yêu cầu đăng nhập"
        subTitle="Bạn cần đăng nhập để truy cập trang này."
        icon={<UserOutlined />}
        extra={
          <Button type="primary" href="#" onClick={() => window.location.reload()}>
            Đăng nhập
          </Button>
        }
      />
    )
  }

  // Check role permissions
  if (requiredRole && isAuthenticated && !hasRole(requiredRole)) {
    const userRole = user?.role || 'Unknown'
    const requiredRoles = Array.isArray(requiredRole) ? requiredRole.join(', ') : requiredRole
    
    return (
      <Result
        status="403"
        title="Không có quyền truy cập"
        subTitle={`Bạn cần có quyền ${requiredRoles} để truy cập trang này. Vai trò hiện tại của bạn: ${userRole}`}
        icon={<LockOutlined />}
        extra={
          <Button type="primary" onClick={() => window.history.back()}>
            Quay lại
          </Button>
        }
      />
    )
  }

  // User has required permissions, render children
  return <>{children}</>
}

export default ProtectedRoute
