# **METAbolique API**
Backend para la aplicación METAbolique

# Flujo de Trabajo en Git

## 1. Desarrollo

Realizar los cambios en la rama `dev2` y probarlos localmente, después realizar los siguientes pasos:

### Pasos

```bash
git add .
# o
git add "nombre_del_archivo"

git commit -m "Mensaje del commit"
git push origin dev2
```

---

## 2. Integración

Una vez que los cambios fueron probados correctamente, hacer merge de `dev2` hacia `dev`.

### Pasos

```bash
git checkout dev
git pull origin dev
git merge dev2
git push origin dev
```

---

## 3. Producción

Cuando los cambios sean aprobados, hacer merge de `dev` hacia `main`.

### Pasos

```bash
git checkout main
git pull origin main
git merge dev
git push origin main
```