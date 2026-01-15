
/**
 * Representa una estructura de datos genérica para respuestas de API paginadas.
 * @template T El tipo de los elementos en la página actual.
 */
export interface PaginatedData<T> {
  items: T[];       // Los datos de la página actual
  totalItems: number; // El número total de items en la base de datos
  itemCount: number;  // El número de items en la página actual
  itemsPerPage: number; // El número de items por página
  totalPages: number; // El número total de páginas
  currentPage: number; // La página actual
}
