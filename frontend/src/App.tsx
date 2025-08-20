import { Outlet } from 'react-router-dom'

export default function App() {
  return (
    <div className="min-h-screen">
      <main className="container mx-auto px-4 py-10">
        <Outlet />
      </main>
    </div>
  )
}
