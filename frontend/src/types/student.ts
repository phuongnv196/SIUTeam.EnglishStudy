export interface Student {
  id: string
  name: string
  email: string
  phone: string
  level: 'Beginner' | 'Intermediate' | 'Advanced'
  status: 'Active' | 'Inactive'
  courses: number
  joinDate: string
  createdAt: string
  updatedAt: string
}

export interface StudentCreateRequest {
  name: string
  email: string
  phone: string
  level: 'Beginner' | 'Intermediate' | 'Advanced'
  status: 'Active' | 'Inactive'
}

export interface StudentUpdateRequest extends Partial<StudentCreateRequest> {
  id: string
}
