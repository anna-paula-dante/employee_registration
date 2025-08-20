
import { useEffect, useState } from 'react'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { employeeSchema, EmployeeInput } from '../../validations/employeeSchemas'
import { employeesService } from '../../services/employeesService'
import { useNavigate, useParams } from 'react-router-dom'
import { useAuthCtx } from '../../context/AuthContext'
import { RoleOrder } from '../../lib/auth'
import { extractApiError } from '../../lib/api'

const roles = [
  { value: 0, label: 'Employee' },
  { value: 1, label: 'Leader' },
  { value: 2, label: 'Director' },
]

export default function EmployeeForm() {
  const nav = useNavigate()
  const { id } = useParams()
  const editing = Boolean(id)
  const { user } = useAuthCtx()
  const [serverError, setServerError] = useState<string | null>(null)

  const { register, control, handleSubmit, reset, watch, formState: { errors, isSubmitting } } = useForm<EmployeeInput>({
    resolver: zodResolver(employeeSchema),
    defaultValues: {
      phones: [{ number: '' }, { number: '' }],
      role: 0
    }
  })
  const { fields, append, remove } = useFieldArray({ control, name: 'phones' })

  useEffect(() => {
    if (editing) {
      employeesService.get(id!).then((e: any) => {
        const roleNumeric = typeof e.role === 'number' ? e.role : (e.role === 'Leader' ? 1 : e.role === 'Director' ? 2 : 0)
        const phonesForForm = Array.isArray(e.phones) ? e.phones.map((p: any) => typeof p === 'string' ? ({ number: p }) : p) : [{ number: '' }, { number: '' }]
        reset({
          firstName: e.firstName,
          lastName: e.lastName,
          email: e.email,
          documentNumber: e.documentNumber,
          birthDate: e.birthDate.substring(0,10),
          role: roleNumeric,
          managerId: e.managerId ?? null,
          phones: phonesForForm
        })
      })
    }
  }, [id, editing, reset])

  const onSubmit = async (data: EmployeeInput) => {
    setServerError(null)
    const myRole = RoleOrder[user!.role]
    const targetRole = data.role
    if (myRole < targetRole) {
      alert('Você não pode criar/alterar alguém com role superior ao seu.')
      return
    }

    const payload: any = {
      firstName: data.firstName,
      lastName: data.lastName,
      email: data.email,
      documentNumber: data.documentNumber,
      birthDate: new Date(data.birthDate).toISOString(),
      role: data.role,
      managerId: data.managerId || null,
      phones: (data.phones || []).map(p => p.number)
    }

    try {
      if (editing) {
        await employeesService.update(id!, payload)
      } else {
        const pwd = (watch('password') || '').trim()
        if (!pwd) {
          alert('Defina uma senha forte para o novo usuário.')
          return
        }
        payload.password = pwd
        await employeesService.create(payload)
      }
      nav('/employees')
    } catch (e) {
      setServerError(extractApiError(e, 'Falha ao salvar'))
    }
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="container-card p-4">
      <h2 className="text-lg font-semibold text-neutral-900 mb-4">{editing ? 'Editar' : 'Novo'} Funcionário</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="label">Nome</label>
          <input className="input" {...register('firstName')} />
          {errors.firstName && <p className="text-sm text-red-600">{errors.firstName.message}</p>}
        </div>
        <div>
          <label className="label">Sobrenome</label>
          <input className="input" {...register('lastName')} />
          {errors.lastName && <p className="text-sm text-red-600">{errors.lastName.message}</p>}
        </div>
        <div>
          <label className="label">Email</label>
          <input className="input" {...register('email')} />
          {errors.email && <p className="text-sm text-red-600">{errors.email.message}</p>}
        </div>
        <div>
          <label className="label">Documento</label>
          <input className="input" {...register('documentNumber')} />
          {errors.documentNumber && <p className="text-sm text-red-600">{errors.documentNumber.message}</p>}
        </div>
        <div>
          <label className="label">Nascimento</label>
          <input type="date" className="input" {...register('birthDate')} />
          {errors.birthDate && <p className="text-sm text-red-600">{errors.birthDate.message}</p>}
        </div>
        <div>
          <label className="label">Role</label>
          <select className="input" {...register('role', { valueAsNumber: true })}>
            {roles.map(r => <option key={r.value} value={r.value}>{r.label}</option>)}
          </select>
          {errors.role && <p className="text-sm text-red-600">{String(errors.role.message)}</p>}
        </div>

        {!editing && (
          <div className="md:col-span-2">
            <label className="label">Senha (apenas na criação)</label>
            <input className="input" type="password" {...register('password')} />
            {errors.password && <p className="text-sm text-red-600">{errors.password.message as any}</p>}
          </div>
        )}

        <div className="md:col-span-2">
          <label className="label">Telefones (mín. 2)</label>
          {fields.map((f, idx) => (
            <div key={f.id} className="flex gap-2 mb-2">
              <input className="input" placeholder="(xx) 9xxxx-xxxx" {...register(`phones.${idx}.number` as const)} />
              <button type="button" className="btn" onClick={() => remove(idx)} disabled={fields.length <= 2}>Remover</button>
            </div>
          ))}
          <button type="button" className="btn" onClick={() => append({ number: '' })}>Adicionar telefone</button>
          {errors.phones && <p className="text-sm text-red-600 mt-1">{errors.phones.message as any}</p>}
        </div>
      </div>

      {serverError && <p className="text-sm text-red-600 mt-3">{serverError}</p>}

      <div className="mt-4 flex gap-2">
        <button type="button" className="btn" onClick={() => nav('/employees')}>Cancelar</button>
        <button type="submit" className="btn btn-primary" disabled={isSubmitting}>{editing ? 'Salvar' : 'Criar'}</button>
      </div>
    </form>
  )
}
