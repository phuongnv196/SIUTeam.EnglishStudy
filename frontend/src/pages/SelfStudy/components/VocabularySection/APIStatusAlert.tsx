import React, { useState, useEffect } from 'react'
import { Alert, Space, Button } from 'antd'
import { ApiOutlined, ExperimentOutlined, ReloadOutlined } from '@ant-design/icons'
import textToSpeechService from '../../../../services/textToSpeechService'
import { useSpeakingHub } from '../../../../hooks/useSpeakingHub'

interface APIStatusAlertProps {
  onOpenTestModal: () => void
  onRefreshData: () => void
  loading: boolean
  onSetDebugToken: () => void
}

const APIStatusAlert: React.FC<APIStatusAlertProps> = ({
  onOpenTestModal,
  onRefreshData,
  loading,
  onSetDebugToken
}) => {
  const [pythonApiHealthy, setPythonApiHealthy] = useState<boolean | null>(null)
  
  // Use speaking hub internally
  const {
    isConnected,
    isConnecting,
    connect,
    disconnect
  } = useSpeakingHub()

  // Check Python API health on component mount
  useEffect(() => {
    const checkHealth = async () => {
      const isHealthy = await textToSpeechService.checkPythonAPIHealth()
      setPythonApiHealthy(isHealthy)
    }
    checkHealth()
  }, [])

  const handleRefreshHealth = async () => {
    const isHealthy = await textToSpeechService.checkPythonAPIHealth()
    setPythonApiHealthy(isHealthy)
  }

  const handleConnect = async () => {
    await connect()
  }

  const handleDisconnect = async () => {
    await disconnect()
  }
  return (
    <Alert
      message={
        <Space>
          <ApiOutlined />
          <span>API Status</span>
        </Space>
      }
      description={
        <div>
          <div>Python API: {pythonApiHealthy === null ? 'Checking...' : pythonApiHealthy ? '✅ Healthy' : '❌ Offline'}</div>
          <div>SignalR Hub: {isConnecting ? 'Connecting...' : isConnected ? '✅ Connected' : '❌ Disconnected'}</div>
          <div style={{ marginTop: '8px' }}>
            <Space>
              <Button 
                size="small" 
                onClick={handleConnect}
                loading={isConnecting}
                disabled={isConnected}
              >
                Connect SignalR
              </Button>
              <Button 
                size="small" 
                onClick={handleDisconnect}
                disabled={!isConnected}
              >
                Disconnect
              </Button>
              <Button 
                size="small" 
                onClick={handleRefreshHealth}
              >
                <ReloadOutlined />
                Refresh API Status
              </Button>
              <Button 
                size="small" 
                type="primary"
                onClick={onOpenTestModal}
              >
                <ExperimentOutlined />
                Test Features
              </Button>
              <Button 
                size="small" 
                icon={<ReloadOutlined />}
                onClick={onRefreshData}
                loading={loading}
              >
                Refresh Data
              </Button>
              <Button 
                size="small" 
                type="dashed"
                onClick={onSetDebugToken}
              >
                🔑 Set Debug Token
              </Button>
            </Space>
          </div>
        </div>
      }
      type={pythonApiHealthy && isConnected ? 'success' : 'warning'}
      showIcon
      style={{ marginBottom: '16px' }}
    />
  )
}

export default APIStatusAlert
