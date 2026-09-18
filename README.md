# rights-vault
Clean Architecture rich entity

## IAM: push de imágenes a ECR

La policy mínima para hacer push de una imagen Docker a ECR está en [`infra/iam/ecr-push-policy.json`](infra/iam/ecr-push-policy.json).

### `ecr:GetAuthorizationToken` con `Resource: "*"`

`GetAuthorizationToken` es una acción **a nivel de cuenta**, no de repositorio. AWS no permite acotarla a un ARN de repositorio concreto; el token sirve para autenticarse contra el registry de ECR de la región antes de cualquier operación de push o pull. Por eso el `Resource` debe ser `"*"`.

### `ecr:BatchGetImage` en un push

`BatchGetImage` no aparece en la documentación oficial de ECR como permiso requerido para push. Se necesita cuando la imagen base (por ejemplo Alpine) genera un **manifest list multi-arquitectura**: ECR usa `BatchGetImage` para resolver ese manifest antes de escribir la imagen final. Sin ese permiso, el push puede fallar en producción de forma silenciosa si copiaste una policy genérica de Stack Overflow que solo incluye las acciones de upload.
