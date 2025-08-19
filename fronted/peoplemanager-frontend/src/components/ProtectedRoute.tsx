
import { Navigate } from 'react-router-dom'
import { useAuthCtx } from '../context/AuthContext'
import { RoleName, RoleOrder } from '../lib/auth'

export default function ProtectedRoute({ children, minRole }: { children: JSX.Element, minRole?: RoleName }) {
  const { user } = useAuthCtx()
  if (!user) return <Navigate to="/login" replace />
  if (minRole && RoleOrder[user.role] < RoleOrder[minRole]) return <Navigate to="/" replace />
  return children
}
