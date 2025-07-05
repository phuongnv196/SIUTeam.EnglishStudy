import React, { useState } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { createBrowserRouter, Navigate, Outlet } from 'react-router-dom'
import type { RouteObject } from 'react-router-dom'
import { ProtectedRoute } from '../components/auth'
import LandingLayout from '../layouts/LandingLayout/LandingLayout'
import MainLayout from '../layouts/Main/MainLayout'
import LandingPage from '../pages/Landing'
import HomePage from '../pages/Home'
import CoursesPage from '../features/courses/pages/CoursesPage'
import ProgressPage from '../pages/Progress'
import QuizPage from '../pages/Quiz'
import SelfStudyPage from '../pages/SelfStudy'
import MicrophoneTestPage from '../pages/MicrophoneTestPageSimple'

// Dashboard Layout Component với navigation
const DashboardLayout: React.FC = () => {
  const [collapsed, setCollapsed] = useState(false)
  const [selectedKey, setSelectedKey] = useState('1')
  const location = useLocation()
  const navigate = useNavigate()

  // Update selected key based on current path
  React.useEffect(() => {
    const path = location.pathname
    if (path.includes('/dashboard/home')) {
      setSelectedKey('1')
    } else if (path.includes('/dashboard/courses')) {
      setSelectedKey('2')
    } else if (path.includes('/dashboard/progress')) {
      setSelectedKey('3')
    } else if (path.includes('/dashboard/quiz')) {
      setSelectedKey('4')
    } else if (path.includes('/dashboard/microphone-test')) {
      setSelectedKey('6')
    } else if (path.includes('/dashboard/self-study')) {
      // Check for specific sections
      if (path.includes('vocabulary')) {
        setSelectedKey('5-1')
      } else if (path.includes('grammar')) {
        setSelectedKey('5-2')
      } else if (path.includes('listening')) {
        setSelectedKey('5-3')
      } else if (path.includes('reading')) {
        setSelectedKey('5-4')
      } else if (path.includes('writing')) {
        setSelectedKey('5-5')
      } else if (path.includes('conversation')) {
        setSelectedKey('5-6')
      } else if (path.includes('combined')) {
        setSelectedKey('5-7')
      } else {
        setSelectedKey('5')
      }
    }
  }, [location.pathname])

  const handleMenuClick = (key: string) => {
    setSelectedKey(key)
    const base = '/dashboard'
    if (key === '1') return navigate(`${base}/home`)
    if (key === '2') return navigate(`${base}/courses`)
    if (key === '3') return navigate(`${base}/progress`)
    if (key === '4') return navigate(`${base}/quiz`)
    if (key === '5') return navigate(`${base}/self-study`)
    if (key === '6') return navigate(`${base}/microphone-test`)
    if (key.startsWith('5-')) {
      const sectionMap: Record<string, string> = {
        '5-1': 'vocabulary',
        '5-2': 'grammar',
        '5-3': 'listening',
        '5-4': 'reading',
        '5-5': 'writing',
        '5-6': 'conversation',
        '5-7': 'combined'
      }
      const section = sectionMap[key]
      if (section) return navigate(`${base}/self-study?section=${section}`)
    }
    navigate(`${base}/home`)
  }

  const handleCollapse = (collapsed: boolean) => {
    setCollapsed(collapsed)
  }

  const handleNotificationClick = () => {
    console.log('Notification clicked')
  }

  return (
    <MainLayout
      selectedKey={selectedKey}
      onMenuClick={handleMenuClick}
      collapsed={collapsed}
      onCollapse={handleCollapse}
      onNotificationClick={handleNotificationClick}
    >
      <Outlet />
    </MainLayout>
  )
}

// Home Page với navigation
const HomePageWithNavigation: React.FC = () => {
  const navigate = useNavigate()
  
  const handleNavigate = (key: string) => {
    switch (key) {
      case '2':
        navigate('/dashboard/courses')
        break
      case '3':
        navigate('/dashboard/progress')
        break
      case '4':
        navigate('/dashboard/quiz')
        break
      case '5':
        navigate('/dashboard/self-study')
        break
      default:
        navigate('/dashboard/home')
    }
  }

  return <HomePage onNavigate={handleNavigate} />
}

// Landing Layout Component
const LandingLayoutWrapper: React.FC = () => {
  return (
    <LandingLayout>
      <Outlet />
    </LandingLayout>
  )
}

const routes: RouteObject[] = [
  {
    path: '/',
    element: <LandingLayoutWrapper />,
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
        <DashboardLayout />
      </ProtectedRoute>
    ),
    children: [
      {
        index: true,
        element: <Navigate to="/dashboard/home" replace />
      },
      {
        path: 'home',
        element: <HomePageWithNavigation />
      },
      {
        path: 'courses',
        element: <CoursesPage />
      },
      {
        path: 'progress',
        element: <ProgressPage />
      },
      {
        path: 'quiz',
        element: <QuizPage />
      },
      {
        path: 'self-study',
        element: <SelfStudyPage />
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
