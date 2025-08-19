
import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { employeesService, EmployeeDto } from '../../services/employeesService'
import { useAuthCtx } from '../../context/AuthContext'

export default function EmployeesList() {
  const [items, setItems] = useState<EmployeeDto[]>([])
  const [search, setSearch] = useState('')
  const [loading, setLoading] = useState(true)
  const nav = useNavigate()
  const { user } = useAuthCtx()

  const load = async () => {
    setLoading(true)
    try {
      const res = await employeesService.list(1, 50, search)
      setItems(res.items as any || [])
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [])

  return (
    <div className="container-card p-4">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-semibold text-neutral-900">Funcionários</h2>
        <button className="btn btn-primary" onClick={() => nav('/employees/new')}>Novo</button>
      </div>
      <div className="mb-3 flex gap-2">
        <input className="input" placeholder="Buscar por nome/email/documento..." value={search} onChange={e=>setSearch(e.target.value)} />
        <button className="btn" onClick={load}>Buscar</button>
      </div>
      {loading ? <p>Carregando...</p> : (
        <div className="overflow-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left text-neutral-600 border-b">
                <th className="py-2">Nome</th>
                <th>Email</th>
                <th>Documento</th>
                <th>Role</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {items.map(e => (
                <tr key={e.id} className="border-b last:border-0 hover:bg-neutral-50">
                  <td className="py-2">{e.firstName} {e.lastName}</td>
                  <td>{e.email}</td>
                  <td>{e.documentNumber}</td>
                  <td><span className="badge">{e.role}</span></td>
                  <td className="text-right">
                    <Link to={`/employees/${e.id}/edit`} className="btn mr-2">Editar</Link>
                    <button className="btn btn-danger" onClick={async()=>{
                      if(confirm('Excluir funcionário?')){
                        await employeesService.remove(e.id)
                        await load()
                      }
                    }}>Excluir</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
