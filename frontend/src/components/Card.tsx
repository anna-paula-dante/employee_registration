type Props = React.PropsWithChildren<{ className?: string; title?: string; subtitle?: string }>

export default function Card({ className = '', title, subtitle, children }: Props) {
  return (
    <section className={`container-card p-6 ${className}`}>
      {(title || subtitle) && (
        <header className="mb-4">
          {title && <h2 className="text-lg font-semibold">{title}</h2>}
          {subtitle && <p className="text-neutral-400 text-sm mt-1">{subtitle}</p>}
        </header>
      )}
      {children}
    </section>
  )
}
