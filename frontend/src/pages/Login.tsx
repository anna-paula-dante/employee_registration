import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useAuthCtx } from '../context/AuthContext'
import { useNavigate } from 'react-router-dom'
import { useState } from 'react'

const schema = z.object({
  emailOrDocument: z.string().min(3, 'Informe email ou documento'),
  password: z.string().min(1, 'Informe a senha')
})
type LoginForm = z.infer<typeof schema>

export default function Login() {
  const { login } = useAuthCtx()
  const nav = useNavigate()
  const [serverError, setServerError] = useState<string | null>(null)

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<LoginForm>({
    resolver: zodResolver(schema)
  })

  const onSubmit = async (data: LoginForm) => {
    setServerError(null)
    try {
      await login(data.emailOrDocument, data.password)
      nav('/employees')
    } catch (err: any) {
      setServerError(err?.message ?? 'Falha no login')
    }
  }

  return (
    <div className="min-h-[80vh] grid place-items-center">
      <form onSubmit={handleSubmit(onSubmit)} className="container-card w-full max-w-md p-7">
        <div className="mb-6">
          <h1 className="text-2xl font-semibold tracking-tight text-brand-900">Entrar</h1>
          <p className="text-brand-500 text-sm mt-1">
            Acesse com suas credenciais corporativas.
          </p>
        </div>

        <div className="mb-4">
          <label className="label">Email ou Documento</label>
          <input className="input" autoComplete="username" {...register('emailOrDocument')} />
          {errors.emailOrDocument && <p className="help">{errors.emailOrDocument.message}</p>}
        </div>

        <div className="mb-2">
          <label className="label">Senha</label>
          <input className="input" type="password" autoComplete="current-password" {...register('password')} />
          {errors.password && <p className="help">{errors.password.message}</p>}
        </div>

        {serverError && <p className="help">{serverError}</p>}

        <button className="btn btn-primary w-full mt-4" disabled={isSubmitting}>
          {isSubmitting ? 'Entrando…' : 'Entrar'}
        </button>

        <p className="text-xs text-brand-500 mt-4 text-center">
          Ambiente seguro — seus dados são protegidos.
        </p>
      </form>
    </div>
  )
}
