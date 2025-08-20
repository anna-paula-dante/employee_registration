
import { api } from '../lib/api'

export type RoleLevel = 0 | 1 | 2

export interface PhoneDto { number: string }
export interface EmployeeDto {
  id: string
  firstName: string
  lastName: string
  email: string
  documentNumber: string
  birthDate: string
  role: RoleLevel
  managerId?: string | null
  phones: PhoneDto[]
}

export interface Paged<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
}

export const employeesService = {
  async list(page=1, pageSize=20, search='') {
    const { data } = await api.get('/api/v1/employees', { params: { page, pageSize, search } })
    return data as Paged<EmployeeDto>
  },
  async get(id: string) {
    const { data } = await api.get(`/api/v1/employees/${id}`)
    return data as EmployeeDto
  },
  async create(input: any) {
    const { data } = await api.post('/api/v1/employees', input)
    return data
  },
  async update(id: string, input: any) {
    const { data } = await api.put(`/api/v1/employees/${id}`, input)
    return data
  },
  async remove(id: string) {
    const { data } = await api.delete(`/api/v1/employees/${id}`)
    return data
  }
}
