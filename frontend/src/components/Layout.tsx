
import { Link, useLocation } from 'react-router-dom'
import { useAuthCtx } from '../context/AuthContext'

export default function Layout({ children }: { children: React.ReactNode }) {
  const { user, logout } = useAuthCtx()
  const loc = useLocation()
  return (
    <div className="min-h-screen bg-neutral-100">
      <header className="bg-white border-b border-neutral-200">
        <div className="max-w-6xl mx-auto px-4 h-14 flex items-center justify-between">
          <div className="flex items-center gap-6">
            <Link to="/" className="font-semibold text-neutral-900">People Manager</Link>
            <nav className="flex gap-4 text-sm">
              <Link className={linkCls(loc.pathname.startsWith('/employees'))} to="/employees">Funcionários</Link>
            </nav>
          </div>
          <div className="flex items-center gap-3 text-sm">
            {user && <span className="badge">{user.name} — {user.role}</span>}
            {user && <button className="btn" onClick={logout}>Sair</button>}
          </div>
        </div>
      </header>
      <main className="max-w-6xl mx-auto p-4">{children}</main>
    </div>
  )
}

function linkCls(active: boolean) {
  return "px-2 py-1 rounded-lg " + (active ? "bg-neutral-200" : "hover:bg-neutral-100")
}
