
import { describe, it, expect } from 'vitest'
import { employeeSchema } from '../validations/employeeSchemas'

describe('Age validation', () => {
  it('rejects under 18', async () => {
    const today = new Date()
    const under = new Date(today.getFullYear() - 17, today.getMonth(), today.getDate()).toISOString().substring(0,10)
    const r = employeeSchema.safeParse({
      firstName: 'A', lastName: 'B', email: 'a@b.com', documentNumber: '12345',
      birthDate: under, role: 0, managerId: null, phones: [{number:'12345678'},{number:'12345679'}]
    })
    expect(r.success).toBe(false)
  })

  it('accepts 18+', async () => {
    const today = new Date()
    const ok = new Date(today.getFullYear() - 20, today.getMonth(), today.getDate()).toISOString().substring(0,10)
    const r = employeeSchema.safeParse({
      firstName: 'A', lastName: 'B', email: 'a@b.com', documentNumber: '12345',
      birthDate: ok, role: 0, managerId: null, phones: [{number:'12345678'},{number:'12345679'}]
    })
    expect(r.success).toBe(true)
  })
})
