export interface User {
  id: string
  username: string
  email: string
  role: 'Admin' | 'Teacher' | 'Student'
  firstName: string
  lastName: string
  avatar?: string
  isActive: boolean
  createdAt: string
  lastLogin?: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  user: User
  token: string
  expiresAt: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
  firstName: string
  lastName: string
}
