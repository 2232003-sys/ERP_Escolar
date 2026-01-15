"use client"
import { useEffect } from 'react'
import { useRouter } from 'next/navigation'

export default function AuthGuard({ children }: { children: React.ReactNode }) {
  const router = useRouter()

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      router.push('/login')
    }
  }, [router])

  const token = typeof window !== 'undefined' ? localStorage.getItem('token') : null

  if (!token) {
    return <div>Redirigiendo al login...</div>
  }

  return <>{children}</>
}