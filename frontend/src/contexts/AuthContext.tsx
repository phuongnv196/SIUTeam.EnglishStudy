import { createContext, useContext, useEffect, useState } from 'react'
import type { ReactNode } from 'react'
import type { User } from '../types/auth'
import AuthService from '../services/authService'
import { message } from 'antd'

interface AuthContextType {
  user: User | null
  isLoading: boolean
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<boolean>
  register: (userData: RegisterData) => Promise<boolean>
  logout: (onSuccess?: () => void) => Promise<void>
  updateUser: (userData: Partial<User>) => Promise<boolean>
  hasRole: (role: string | string[]) => boolean
  refreshUserData: () => Promise<void>
}

interface RegisterData {
  username: string
  email: string
  password: string
  firstName: string
  lastName: string
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

interface AuthProviderProps {
  children: ReactNode
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  // Initialize auth state
  useEffect(() => {
    const initAuth = async () => {
      try {
        const storedUser = AuthService.getUser()
        if (storedUser && AuthService.isAuthenticated()) {
          console.log('Stored user:', storedUser) // Debug log
          // Verify token with server
          const currentUser = await AuthService.getCurrentUser()
          if (currentUser) {
            console.log('Current user from server:', currentUser) // Debug log
            setUser(currentUser)
          } else {
            AuthService.clearAuthData()
          }
        }
      } catch (error) {
        console.error('Auth initialization error:', error)
        AuthService.clearAuthData()
      } finally {
        setIsLoading(false)
      }
    }

    initAuth()
  }, [])

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      setIsLoading(true)
      const response = await AuthService.login({ email, password })
      console.log('Login response user:', response.user) // Debug log
      setUser(response.user)
      message.success(`Chào mừng ${response.user.firstName}!`)
      return true
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Đăng nhập thất bại'
      message.error(errorMessage)
      return false
    } finally {
      setIsLoading(false)
    }
  }

  const register = async (userData: RegisterData): Promise<boolean> => {
    try {
      setIsLoading(true)
      await AuthService.register(userData)
      message.success('Đăng ký thành công! Vui lòng đăng nhập để tiếp tục.')
      return true
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Đăng ký thất bại'
      message.error(errorMessage)
      return false
    } finally {
      setIsLoading(false)
    }
  }

  const logout = async (onSuccess?: () => void): Promise<void> => {
    try {
      setIsLoading(true)
      await AuthService.logout()
      setUser(null)
      message.success('Đã đăng xuất thành công')
      // Call the callback if provided (for redirect logic)
      if (onSuccess) {
        onSuccess()
      }
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      setIsLoading(false)
    }
  }

  const updateUser = async (userData: Partial<User>): Promise<boolean> => {
    try {
      const updatedUser = await AuthService.updateProfile(userData)
      setUser(updatedUser)
      message.success('Cập nhật thông tin thành công')
      return true
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Cập nhật thất bại'
      message.error(errorMessage)
      return false
    }
  }

  const hasRole = (role: string | string[]): boolean => {
    return AuthService.hasRole(role)
  }

  const refreshUserData = async (): Promise<void> => {
    try {
      const currentUser = await AuthService.getCurrentUser()
      if (currentUser) {
        setUser(currentUser)
      }
    } catch (error) {
      console.error('Refresh user data error:', error)
    }
  }

  const isAuthenticated = !!user && AuthService.isAuthenticated()

  const value: AuthContextType = {
    user,
    isLoading,
    isAuthenticated,
    login,
    register,
    logout,
    updateUser,
    hasRole,
    refreshUserData,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

export default AuthContext
