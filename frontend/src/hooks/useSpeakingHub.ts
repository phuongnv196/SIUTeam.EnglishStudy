import { useState, useEffect, useCallback, useRef } from 'react'
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
import { message } from 'antd'

interface SpeakingSessionData {
  sessionId: string
  expectedText: string
  message: string
}

interface ChunkTranscriptionData {
  sessionId: string
  transcribedText: string
  processingTime: number
  language: string
  ipa: {
    g2pIpa: string
    epitranIpa: string
    arpabet: string
    success: boolean
  }
}

interface PronunciationFeedbackData {
  originalText: string
  ipaNotation: string
  arpabet: string
  success: boolean
}

interface SpeakingSessionEndData {
  sessionId: string
  transcribedText: string
  expectedText: string
  confidenceScore: number
  processingTime: number
  ipa: {
    g2pIpa: string
    epitranIpa: string
    arpabet: string
    success: boolean
  }
  suggestions: string[]
}

interface TalkingAvatarData {
  success: boolean
  text: string
  ipaText: string
  arpabet: string
  processingTime: number
  animation: {
    duration: number
    fps: number
    totalFrames: number
    visemesCount: number
    phonemesCount: number
    keyframes: Array<{
      frame: number
      time: number
      viseme: string
      weight: number
    }>
    visemes: Array<{
      name: string
      startTime: number
      duration: number
    }>
  }
}

export const useSpeakingHub = () => {
  const [connection, setConnection] = useState<HubConnection | null>(null)
  const [isConnected, setIsConnected] = useState(false)
  const [isConnecting, setIsConnecting] = useState(false)
  const [activeSessionId, setActiveSessionId] = useState<string | null>(null)
  const [pronunciationFeedback, setPronunciationFeedback] = useState<PronunciationFeedbackData | null>(null)
  const [lastTranscription, setLastTranscription] = useState<string>('')
  const [talkingAvatarData, setTalkingAvatarData] = useState<TalkingAvatarData | null>(null)

  const connectionRef = useRef<HubConnection | null>(null)

  const connect = useCallback(async (token?: string) => {
    if (connectionRef.current?.state === 'Connected') {
      return
    }

    setIsConnecting(true)
    
    try {
      // Get token from parameter or localStorage
      const authToken = token || localStorage.getItem('token')
      
      let hubUrl = 'http://localhost:5073/hubs/speaking'
      
      const connectionBuilder = new HubConnectionBuilder()
        .withUrl(hubUrl, {
            accessTokenFactory: () => authToken || '', // Use token if available
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)

      const newConnection = connectionBuilder.build()

      // Event handlers
      newConnection.on('SpeakingSessionStarted', (data: SpeakingSessionData) => {
        console.log('Speaking session started:', data)
        setActiveSessionId(data.sessionId)
        message.success(`Session started: ${data.sessionId}`)
      })

      newConnection.on('ChunkTranscribed', (data: ChunkTranscriptionData) => {
        console.log('Chunk transcribed:', data)
        setLastTranscription(data.transcribedText)
        if (data.transcribedText) {
          message.info(`Transcribed: ${data.transcribedText}`)
        }
      })

      newConnection.on('SpeakingSessionEnded', (data: SpeakingSessionEndData) => {
        console.log('Speaking session ended:', data)
        setActiveSessionId(null)
        message.success(`Session ended. Confidence: ${(data.confidenceScore * 100).toFixed(1)}%`)
      })

      newConnection.on('PronunciationFeedback', (data: PronunciationFeedbackData) => {
        console.log('Pronunciation feedback:', data)
        setPronunciationFeedback(data)
        message.success('Pronunciation feedback received')
      })

      newConnection.on('TalkingAvatarCreated', (data: TalkingAvatarData) => {
        console.log('Talking avatar created:', data)
        setTalkingAvatarData(data)
        message.success(`Avatar created: ${data.animation.totalFrames} frames`)
      })

      newConnection.on('SpeakingError', (error: { error: string, details?: string }) => {
        console.error('Speaking error:', error)
        message.error(`Speaking error: ${error.error}`)
      })

      newConnection.onreconnecting(() => {
        console.log('SignalR reconnecting...')
        setIsConnected(false)
      })

      newConnection.onreconnected(() => {
        console.log('SignalR reconnected')
        setIsConnected(true)
        message.success('Reconnected to speaking service')
      })

      newConnection.onclose(() => {
        console.log('SignalR connection closed')
        setIsConnected(false)
        setActiveSessionId(null)
      })

      await newConnection.start()
      
      connectionRef.current = newConnection
      setConnection(newConnection)
      setIsConnected(true)
      message.success('Connected to speaking service')
      
    } catch (error) {
      console.error('Error connecting to SignalR hub:', error)
      message.error('Failed to connect to speaking service')
    } finally {
      setIsConnecting(false)
    }
  }, [])

  const disconnect = useCallback(async () => {
    if (connectionRef.current) {
      try {
        await connectionRef.current.stop()
      } catch (error) {
        console.error('Error disconnecting:', error)
      }
      connectionRef.current = null
      setConnection(null)
      setIsConnected(false)
      setActiveSessionId(null)
    }
  }, [])

  const startSpeakingSession = useCallback(async (lessonId: string, expectedText: string) => {
    if (!connection || !isConnected) {
      message.error('Not connected to speaking service')
      return false
    }

    try {
      await connection.invoke('StartSpeakingSession', lessonId, expectedText)
      return true
    } catch (error) {
      console.error('Error starting speaking session:', error)
      message.error('Failed to start speaking session')
      return false
    }
  }, [connection, isConnected])

  const endSpeakingSession = useCallback(async (finalAudio: string, expectedText: string) => {
    if (!connection || !isConnected || !activeSessionId) {
      message.error('No active session')
      return false
    }

    try {
      await connection.invoke('EndSpeakingSession', activeSessionId, finalAudio, expectedText)
      return true
    } catch (error) {
      console.error('Error ending speaking session:', error)
      message.error('Failed to end speaking session')
      return false
    }
  }, [connection, isConnected, activeSessionId])

  const processAudioChunk = useCallback(async (audioChunk: string) => {
    if (!connection || !isConnected || !activeSessionId) {
      return false
    }

    try {
      await connection.invoke('ProcessAudioChunk', activeSessionId, audioChunk)
      return true
    } catch (error) {
      console.error('Error processing audio chunk:', error)
      return false
    }
  }, [connection, isConnected, activeSessionId])

  const getPronunciationFeedback = useCallback(async (text: string) => {
    if (!connection || !isConnected) {
      message.error('Not connected to speaking service')
      return false
    }

    try {
      await connection.invoke('GetPronunciationFeedback', text)
      return true
    } catch (error) {
      console.error('Error getting pronunciation feedback:', error)
      message.error('Failed to get pronunciation feedback')
      return false
    }
  }, [connection, isConnected])

  const createTalkingAvatar = useCallback(async (text: string, duration: number = 3.0, fps: number = 30) => {
    if (!connection || !isConnected) {
      message.error('Not connected to speaking service')
      return false
    }

    try {
      await connection.invoke('CreateTalkingAvatar', text, duration, fps)
      return true
    } catch (error) {
      console.error('Error creating talking avatar:', error)
      message.error('Failed to create talking avatar')
      return false
    }
  }, [connection, isConnected])

//   // Auto-connect on mount (always try to connect with token if available)
//   useEffect(() => {
//     const token = localStorage.getItem('token')
//     if (!connection) {
//       // Try to connect with token if available, otherwise connect anonymously for testing
//       connect(token || undefined)
//     }

//     return () => {
//       disconnect()
//     }
//   }, [connection, connect, disconnect])

  // Cleanup on unmount
  useEffect(() => {
    return () => {
      if (connectionRef.current) {
        connectionRef.current.stop()
      }
    }
  }, [])

  return {
    // Connection state
    isConnected,
    isConnecting,
    activeSessionId,
    
    // Connection methods
    connect,
    disconnect,
    
    // Speaking methods
    startSpeakingSession,
    endSpeakingSession,
    processAudioChunk,
    getPronunciationFeedback,
    createTalkingAvatar,
    
    // Data
    pronunciationFeedback,
    lastTranscription,
    talkingAvatarData,
    
    // Clear data methods
    clearPronunciationFeedback: () => setPronunciationFeedback(null),
    clearLastTranscription: () => setLastTranscription(''),
    clearTalkingAvatarData: () => setTalkingAvatarData(null)
  }
}
