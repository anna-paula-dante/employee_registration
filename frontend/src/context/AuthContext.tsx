import { createContext, useContext, useState } from 'react'
import { api, extractApiError } from '../lib/api'

type User = { id: string; name: string; email: string; role: string }
type Ctx = {
  user: User | null
  token: string | null
  login: (emailOrDocument: string, password: string) => Promise<void>
  logout: () => void
}

const Ctx = createContext<Ctx>(null as any)

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'))
  const [user, setUser] = useState<User | null>(null)

  const login = async (emailOrDocument: string, password: string) => {
    try {
      // não envia Authorization (o interceptor já garante); limpa token velho
      localStorage.removeItem('token')

      const { data } = await api.post('/api/v1/auth/login', {
        emailOrDocument: emailOrDocument.trim(),
        password
      })

      const t = data.access_token as string
      localStorage.setItem('token', t)
      setToken(t)
      setUser(data.user as User)
    } catch (e) {
      throw new Error(extractApiError(e))
    }
  }

  const logout = () => {
    localStorage.removeItem('token')
    setToken(null)
    setUser(null)
  }

  return <Ctx.Provider value={{ user, token, login, logout }}>{children}</Ctx.Provider>
}

export const useAuthCtx = () => useContext(Ctx)
