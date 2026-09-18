# Arquitectura de RightsVault

## Agregados y fronteras

1. `LicenseAgreement` es raíz de agregado porque controla el ciclo de vida completo de una licencia.
2. Su frontera protege que toda licencia tenga título y un periodo válido antes de existir.
3. `DateRange` vive dentro de `LicenseAgreement`; no es agregado porque carece de identidad y ciclo de vida propio.
4. `Right` es raíz de agregado porque una modalidad de explotación debe conservar una definición única y consistente.
5. `Right` y `LicenseAgreement` están separados porque un derecho puede catalogarse antes de ser concedido en una licencia.
6. `Territory` es raíz de agregado porque su código, nombre y jerarquía geográfica deben ser válidos por sí mismos.
7. `Territory` y `LicenseAgreement` están separados porque el territorio existe aunque ninguna licencia lo utilice.
8. Una licencia referencia derechos y territorios por identidad, sin modificar sus definiciones internas.
9. La exclusividad cruza licencias porque ninguna licencia exclusiva puede solaparse en derecho, territorio y periodo.
10. Esa invariante se valida consultando otros agregados, sin convertir todas las licencias en un único agregado gigante.
