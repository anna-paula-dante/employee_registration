
import { z } from 'zod'

export const passwordSchema = z.string().min(8).regex(/[A-Z]/, '1 maiúscula').regex(/[a-z]/, '1 minúscula').regex(/[0-9]/, '1 dígito').regex(/[^A-Za-z0-9]/, '1 caractere especial')

export const employeeSchema = z.object({
  firstName: z.string().min(1, 'Obrigatório'),
  lastName: z.string().min(1, 'Obrigatório'),
  email: z.string().email('Email inválido'),
  documentNumber: z.string().min(5, 'Obrigatório'),
  birthDate: z.string().refine((iso) => {
    const d = new Date(iso)
    if (isNaN(d.valueOf())) return false
    const today = new Date()
    const age = today.getFullYear() - d.getFullYear() - (today < new Date(today.getFullYear(), d.getMonth(), d.getDate()) ? 1 : 0)
    return age >= 18
  }, 'Necessário ser maior de idade'),
  password: passwordSchema.optional(), // obrigatória só na criação (tratado no form)
  role: z.number().int().min(0).max(2),
  managerId: z.string().uuid().nullable().optional(),
  phones: z.array(z.object({ number: z.string().min(8, 'Telefone inválido') })).min(2, 'Mínimo 2 telefones')
})

export type EmployeeInput = z.infer<typeof employeeSchema>
