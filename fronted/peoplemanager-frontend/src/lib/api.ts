import axios, { AxiosError } from 'axios'

const BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:8080'

export const api = axios.create({
  baseURL: BASE_URL,
  withCredentials: false
})

// Interceptor de request: injeta Bearer EXCETO em /auth/login
api.interceptors.request.use((cfg) => {
  const isLogin = (cfg.url ?? '').includes('/api/v1/auth/login')
  if (!isLogin) {
    const token = localStorage.getItem('token')
    if (token) cfg.headers.Authorization = `Bearer ${token}`
  } else {
    // garante que não vamos com um token velho no cabeçalho por engano
    if (cfg.headers && 'Authorization' in cfg.headers) {
      delete (cfg.headers as any).Authorization
    }
  }
  return cfg
})

// Interceptor de response: se 401/403, limpa token (sessão expirada ou inválida)
api.interceptors.response.use(
  (res) => res,
  (err: AxiosError<any>) => {
    const status = err.response?.status
    if (status === 401 || status === 403) {
      localStorage.removeItem('token')
    }
    return Promise.reject(err)
  }
)

// Helper para extrair mensagem de erro amigável do backend
export function extractApiError(e: unknown, fallback = 'Falha no login') {
  const ax = e as AxiosError<any>
  return (
    ax?.response?.data?.message ||
    ax?.response?.data?.error ||
    ax?.message ||
    fallback
  )
}
