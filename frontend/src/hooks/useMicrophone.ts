import { useState, useRef, useCallback } from 'react'
import { message } from 'antd'

interface UseMicrophoneOptions {
  onDataAvailable?: (audioBlob: Blob) => void
  onChunkAvailable?: (audioChunk: Blob) => void
  chunkInterval?: number // milliseconds
  mimeType?: string
  enhancedNoiseReduction?: boolean // Use enhanced browser noise reduction
}

type RecordingState = 'inactive' | 'recording' | 'paused'

export const useMicrophone = (options: UseMicrophoneOptions = {}) => {
  const {
    onDataAvailable,
    onChunkAvailable,
    chunkInterval = 1000,
    mimeType = 'audio/webm;codecs=opus',
    enhancedNoiseReduction = true // Default to true for better audio quality
  } = options

  const [isRecording, setIsRecording] = useState(false)
  const [isPermissionGranted, setIsPermissionGranted] = useState<boolean | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [audioLevel, setAudioLevel] = useState(0) // Audio level 0-100

  const mediaRecorderRef = useRef<MediaRecorder | null>(null)
  const streamRef = useRef<MediaStream | null>(null)
  const audioContextRef = useRef<AudioContext | null>(null)
  const analyserRef = useRef<AnalyserNode | null>(null)
  const animationFrameRef = useRef<number | null>(null)
  const audioChunksRef = useRef<Blob[]>([])

  // Request microphone permission
  const requestPermission = useCallback(async (): Promise<boolean> => {
    try {
      setError(null)
      
      // Check if already have permission
      const permissionStatus = await navigator.permissions.query({ name: 'microphone' as PermissionName })
      
      if (permissionStatus.state === 'granted') {
        setIsPermissionGranted(true)
        return true
      }

      // Request microphone access with basic settings
      const stream = await navigator.mediaDevices.getUserMedia({ 
        audio: {
          echoCancellation: true,
          noiseSuppression: true,
          autoGainControl: true,
          sampleRate: 44100
        } 
      })
      
      // Permission granted
      setIsPermissionGranted(true)
      
      // Stop the stream since we just wanted to check permission
      stream.getTracks().forEach(track => track.stop())
      
      message.success('Microphone permission granted')
      return true
      
    } catch (error) {
      console.error('Error requesting microphone permission:', error)
      setIsPermissionGranted(false)
      
      if (error instanceof Error) {
        setError(error.message)
        
        if (error.name === 'NotAllowedError') {
          message.error('Microphone permission denied. Please allow access in browser settings.')
        } else if (error.name === 'NotFoundError') {
          message.error('No microphone found. Please connect a microphone.')
        } else {
          message.error('Error accessing microphone: ' + error.message)
        }
      } else {
        setError('Unknown error accessing microphone')
        message.error('Unknown error accessing microphone')
      }
      
      return false
    }
  }, [])

  // Setup audio level monitoring
  const setupAudioLevelMonitoring = useCallback((stream: MediaStream) => {
    try {
      audioContextRef.current = new AudioContext()
      analyserRef.current = audioContextRef.current.createAnalyser()
      
      const source = audioContextRef.current.createMediaStreamSource(stream)
      source.connect(analyserRef.current)
      
      analyserRef.current.fftSize = 256
      const bufferLength = analyserRef.current.frequencyBinCount
      const dataArray = new Uint8Array(bufferLength)
      
      const updateAudioLevel = () => {
        if (analyserRef.current && isRecording) {
          analyserRef.current.getByteFrequencyData(dataArray)
          
          // Calculate average audio level
          const sum = dataArray.reduce((acc, value) => acc + value, 0)
          const average = sum / bufferLength
          const level = Math.round((average / 255) * 100)
          
          setAudioLevel(level)
          animationFrameRef.current = requestAnimationFrame(updateAudioLevel)
        }
      }
      
      updateAudioLevel()
      
    } catch (error) {
      console.error('Error setting up audio level monitoring:', error)
    }
  }, [isRecording])

  // Start recording
  const startRecording = useCallback(async (): Promise<boolean> => {
    try {
      setError(null)
      
      // Request permission if not granted
      if (!isPermissionGranted) {
        const granted = await requestPermission()
        if (!granted) return false
      }

      // Get media stream with browser noise reduction
      const audioConstraints = enhancedNoiseReduction ? {
        // Enhanced browser noise reduction settings
        echoCancellation: true,
        noiseSuppression: true,
        autoGainControl: true,
        sampleRate: 44100,
        channelCount: 1,
        // Additional Google Chrome specific features (if available)
        googEchoCancellation: true,
        googNoiseSuppression: true,
        googAutoGainControl: true,
        googHighpassFilter: true,
        googTypingNoiseDetection: true,
        googNoiseReduction: true
      } : {
        // Standard browser noise reduction
        echoCancellation: true,
        noiseSuppression: true,
        autoGainControl: true,
        sampleRate: 44100,
        channelCount: 1
      }

      const stream = await navigator.mediaDevices.getUserMedia({ 
        audio: audioConstraints
      })
      
      streamRef.current = stream
      
      // Setup audio level monitoring
      setupAudioLevelMonitoring(stream)
      
      // Create MediaRecorder
      const mediaRecorder = new MediaRecorder(stream, {
        mimeType: mimeType
      })
      
      mediaRecorderRef.current = mediaRecorder
      audioChunksRef.current = []
      
      // Handle data available
      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          audioChunksRef.current.push(event.data)
          
          // Call chunk callback if provided
          if (onChunkAvailable) {
            onChunkAvailable(event.data)
          }
        }
      }
      
      // Handle recording stop
      mediaRecorder.onstop = () => {
        const audioBlob = new Blob(audioChunksRef.current, { type: mimeType })
        
        if (onDataAvailable) {
          onDataAvailable(audioBlob)
        }
        
        // Cleanup
        setIsRecording(false)
        setAudioLevel(0)
        
        if (animationFrameRef.current) {
          cancelAnimationFrame(animationFrameRef.current)
          animationFrameRef.current = null
        }
        
        if (audioContextRef.current && audioContextRef.current.state !== 'closed') {
          audioContextRef.current.close()
          audioContextRef.current = null
        }
      }
      
      // Start recording
      setIsRecording(true)
      mediaRecorder.start(chunkInterval)
      
      const noiseReductionStatus = enhancedNoiseReduction ? 'with enhanced noise reduction' : 'with standard settings'
      message.success(`Recording started ${noiseReductionStatus}`)
      return true
      
    } catch (error) {
      console.error('Error starting recording:', error)
      setIsRecording(false)
      
      if (error instanceof Error) {
        setError(error.message)
        message.error('Error starting recording: ' + error.message)
      } else {
        setError('Unknown error starting recording')
        message.error('Unknown error starting recording')
      }
      
      return false
    }
  }, [
    isPermissionGranted, 
    requestPermission, 
    setupAudioLevelMonitoring,
    enhancedNoiseReduction,
    mimeType, 
    chunkInterval, 
    onChunkAvailable, 
    onDataAvailable
  ])

  // Stop recording
  const stopRecording = useCallback((): Promise<Blob | null> => {
    return new Promise((resolve) => {
      if (mediaRecorderRef.current && mediaRecorderRef.current.state === 'recording') {
        // Set up one-time event listener for the final blob
        const handleStop = () => {
          const audioBlob = new Blob(audioChunksRef.current, { type: mimeType })
          resolve(audioBlob)
        }
        
        mediaRecorderRef.current.addEventListener('stop', handleStop, { once: true })
        mediaRecorderRef.current.stop()
        
        // Stop stream tracks
        if (streamRef.current) {
          streamRef.current.getTracks().forEach(track => track.stop())
          streamRef.current = null
        }
        
        message.info('Recording stopped')
      } else {
        resolve(null)
      }
    })
  }, [mimeType])

  // Pause recording
  const pauseRecording = useCallback(() => {
    if (mediaRecorderRef.current && mediaRecorderRef.current.state === 'recording') {
      mediaRecorderRef.current.pause()
      setIsRecording(false)
      message.info('Recording paused')
    }
  }, [])

  // Resume recording
  const resumeRecording = useCallback(() => {
    if (mediaRecorderRef.current && mediaRecorderRef.current.state === 'paused') {
      mediaRecorderRef.current.resume()
      setIsRecording(true)
      message.info('Recording resumed')
    }
  }, [])

  // Get recording state
  const getRecordingState = useCallback((): RecordingState => {
    if (!mediaRecorderRef.current) return 'inactive'
    return mediaRecorderRef.current.state
  }, [])

  // Check if microphone is supported
  const isSupported = useCallback((): boolean => {
    return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia)
  }, [])

  // Convert blob to base64
  const blobToBase64 = useCallback((blob: Blob): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader()
      reader.onloadend = () => {
        const result = reader.result as string
        // Remove data:audio/webm;base64, prefix
        const base64 = result.split(',')[1]
        resolve(base64)
      }
      reader.onerror = reject
      reader.readAsDataURL(blob)
    })
  }, [])

  // Convert blob to array buffer
  const blobToArrayBuffer = useCallback((blob: Blob): Promise<ArrayBuffer> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader()
      reader.onloadend = () => resolve(reader.result as ArrayBuffer)
      reader.onerror = reject
      reader.readAsArrayBuffer(blob)
    })
  }, [])

  // Cleanup on unmount
  const cleanup = useCallback(() => {
    if (mediaRecorderRef.current && mediaRecorderRef.current.state === 'recording') {
      mediaRecorderRef.current.stop()
    }
    
    if (streamRef.current) {
      streamRef.current.getTracks().forEach(track => track.stop())
      streamRef.current = null
    }
    
    if (animationFrameRef.current) {
      cancelAnimationFrame(animationFrameRef.current)
      animationFrameRef.current = null
    }
    
    if (audioContextRef.current && audioContextRef.current.state !== 'closed') {
      audioContextRef.current.close()
      audioContextRef.current = null
    }
    
    console.log('Microphone resources cleaned up')
  }, [])

  return {
    // State
    isRecording,
    isPermissionGranted,
    error,
    audioLevel,
    
    // Enhanced features
    isEnhancedNoiseReductionEnabled: enhancedNoiseReduction,
    
    // Actions
    requestPermission,
    startRecording,
    stopRecording,
    pauseRecording,
    resumeRecording,
    cleanup,
    
    // Utilities
    getRecordingState,
    isSupported,
    blobToBase64,
    blobToArrayBuffer
  }
}

export default useMicrophone
