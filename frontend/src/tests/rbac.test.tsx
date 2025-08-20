
import { describe, it, expect } from 'vitest'
import { RoleOrder } from '../lib/auth'

describe('RBAC order', () => {
  it('Director >= Leader >= Employee', () => {
    expect(RoleOrder.Director).toBeGreaterThan(RoleOrder.Leader)
    expect(RoleOrder.Leader).toBeGreaterThan(RoleOrder.Employee)
  })
})
