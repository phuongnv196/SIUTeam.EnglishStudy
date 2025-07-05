import { createBrowserRouter, Navigate } from 'react-router-dom'
import type { RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '../components/auth'
import LandingLayout from '../layouts/LandingLayout/LandingLayout'
import MainLayout from '../layouts/Main/MainLayout'
import LandingPage from '../pages/Landing'
import HomePage from '../pages/Home'
import CoursesPage from '../features/courses/pages/CoursesPage'
import MicrophoneTestPage from '../pages/MicrophoneTestPageSimple'
import { Outlet } from 'react-router-dom'

const routes: RouteObject[] = [
  {
    path: '/',
    element: <LandingLayout><Outlet /></LandingLayout>,
    children: [
      {
        index: true,
        element: <LandingPage />
      }
    ]
  },
  {
    path: '/microphone-test',
    element: <MicrophoneTestPage />
  },
  {
    path: '/dashboard',
    element: (
      <ProtectedRoute>
        <MainLayout 
          selectedKey="1" 
          onMenuClick={() => {}} 
          collapsed={false} 
          onCollapse={() => {}}
          onNotificationClick={() => {}}
        >
          <Outlet />
        </MainLayout>
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <Navigate to="/dashboard/home" replace />
      },
      {
        path: 'home',
        element: <HomePage onNavigate={() => {}} />
      },
      {
        path: 'courses',
        element: <CoursesPage />
      },
      {
        path: 'microphone-test',
        element: <MicrophoneTestPage />
      }
    ]
  },
  // Fallback route
  {
    path: '*',
    element: <Navigate to="/" replace />
  }
]

export const router = createBrowserRouter(routes)

export default router
