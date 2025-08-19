
export type RoleName = 'Employee' | 'Leader' | 'Director'

export const RoleOrder: Record<RoleName, number> = {
  Employee: 0,
  Leader: 1,
  Director: 2
}

export function asRoleName(role: number | string): RoleName {
  const numericMode = String(import.meta.env.VITE_API_ROLE_IS_NUMERIC ?? 'true') === 'true'
  if (numericMode) {
    const map: Record<number, RoleName> = { 0: 'Employee', 1: 'Leader', 2: 'Director' }
    return map[Number(role)] ?? 'Employee'
  }
  const s = String(role)
  if (s === 'Leader' || s === 'Director') return s
  return 'Employee'
}

export interface CurrentUser {
  id: string
  name: string
  email: string
  role: RoleName
}
