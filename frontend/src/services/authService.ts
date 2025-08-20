
import { api } from '../lib/api'

export const authService = {
  async login(emailOrDocument: string, password: string) {
    const { data } = await api.post('/api/v1/auth/login', { emailOrDocument, password })
    return data as {
      access_token: string
      token_type: string
      expires_at: string
      user: { id: string, name: string, email: string, role: number | string }
    }
  }
}
