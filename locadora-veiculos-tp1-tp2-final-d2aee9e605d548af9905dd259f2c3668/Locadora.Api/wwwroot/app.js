'use strict';

const resources = {
  fabricantes: {
    title: 'Fabricantes', description: 'Marcas responsáveis pelos veículos da frota.',
    singular: 'fabricante', filters: [{ key: 'nome', label: 'Nome do fabricante', placeholder: 'Buscar por nome' }, { key: 'pais', label: 'País de origem', placeholder: 'Buscar por país' }],
    columns: [{ key: 'id', label: 'ID' }, { key: 'nome', label: 'Fabricante' }, { key: 'pais', label: 'País' }],
    fields: [{ key: 'nome', label: 'Nome', required: true, max: 100, full: true }, { key: 'pais', label: 'País', required: true, max: 80, full: true }]
  },
  veiculos: {
    title: 'Veículos', description: 'Inventário da frota, quilometragem e valor da diária.',
    singular: 'veículo', filters: [{ key: 'modelo', label: 'Modelo', placeholder: 'Buscar por modelo' }, { key: 'placa', label: 'Placa', placeholder: 'Buscar por placa' }],
    columns: [{ key: 'id', label: 'ID' }, { key: 'modelo', label: 'Modelo' }, { key: 'placa', label: 'Placa' }, { key: 'fabricante', label: 'Fabricante' }, { key: 'anoFabricacao', label: 'Ano' }, { key: 'quilometragem', label: 'Quilometragem', format: 'km' }, { key: 'valorDiaria', label: 'Diária', format: 'money' }],
    fields: [{ key: 'fabricanteId', label: 'Fabricante', type: 'select', source: 'fabricantes', required: true, full: true }, { key: 'modelo', label: 'Modelo', required: true, max: 100 }, { key: 'placa', label: 'Placa', required: true, max: 7, pattern: '[A-Za-z]{3}[0-9][A-Za-z0-9][0-9]{2}', hint: 'Ex.: ABC1D23' }, { key: 'anoFabricacao', label: 'Ano de fabricação', type: 'number', min: 1900, max: 2100, required: true }, { key: 'quilometragem', label: 'Quilometragem', type: 'number', min: 0, required: true }, { key: 'valorDiaria', label: 'Valor da diária (R$)', type: 'number', min: 0.01, step: 0.01, required: true }]
  },
  clientes: {
    title: 'Clientes', description: 'Pessoas cadastradas para realizar locações.',
    singular: 'cliente', filters: [{ key: 'nome', label: 'Nome do cliente', placeholder: 'Buscar por nome' }, { key: 'email', label: 'E-mail', placeholder: 'Buscar por e-mail' }],
    columns: [{ key: 'id', label: 'ID' }, { key: 'nome', label: 'Nome' }, { key: 'cpf', label: 'CPF', format: 'cpf' }, { key: 'email', label: 'E-mail' }],
    fields: [{ key: 'nome', label: 'Nome completo', required: true, max: 120, full: true }, { key: 'cpf', label: 'CPF (somente números)', required: true, max: 11, pattern: '[0-9]{11}', hint: '11 dígitos; o backend valida os dígitos verificadores.' }, { key: 'email', label: 'E-mail', type: 'email', required: true, max: 200 }]
  },
  alugueis: {
    title: 'Aluguéis', description: 'Períodos, devoluções, quilômetros e valores.',
    singular: 'aluguel', filters: [{ key: 'cliente', label: 'Cliente', placeholder: 'Buscar por cliente' }, { key: 'veiculo', label: 'Veículo', placeholder: 'Buscar por veículo' }],
    columns: [{ key: 'id', label: 'ID' }, { key: 'cliente', label: 'Cliente' }, { key: 'veiculo', label: 'Veículo' }, { key: 'inicio', label: 'Início', format: 'date' }, { key: 'fimPrevisto', label: 'Fim previsto', format: 'date' }, { key: 'devolucao', label: 'Situação', format: 'status' }, { key: 'valorTotal', label: 'Total', format: 'money' }],
    fields: [{ key: 'clienteId', label: 'Cliente', type: 'select', source: 'clientes', required: true, full: true }, { key: 'veiculoId', label: 'Veículo', type: 'select', source: 'veiculos', required: true, full: true }, { key: 'inicio', label: 'Início', type: 'datetime-local', required: true }, { key: 'fimPrevisto', label: 'Fim previsto', type: 'datetime-local', required: true }]
  },
  pagamentos: {
    title: 'Pagamentos', description: 'Valores recebidos por aluguel devolvido.',
    singular: 'pagamento', filters: [{ key: 'forma', label: 'Forma de pagamento', placeholder: 'Buscar por forma' }, { key: 'aluguelId', label: 'ID do aluguel', placeholder: 'Buscar por aluguel' }],
    columns: [{ key: 'id', label: 'ID' }, { key: 'aluguelId', label: 'Aluguel' }, { key: 'data', label: 'Data', format: 'date' }, { key: 'valor', label: 'Valor', format: 'money' }, { key: 'forma', label: 'Forma' }],
    fields: [{ key: 'aluguelId', label: 'Aluguel devolvido', type: 'select', source: 'alugueis', required: true, full: true }, { key: 'data', label: 'Data do pagamento', type: 'datetime-local', required: true }, { key: 'valor', label: 'Valor (R$)', type: 'number', min: 0.01, step: 0.01, required: true }, { key: 'forma', label: 'Forma', type: 'choice', options: ['Pix', 'Cartao', 'Dinheiro', 'Transferencia'], required: true }]
  }
};

const queries = [
  { key: 'veiculos-por-fabricante', title: 'Veículos por fabricante', description: 'Encontre os modelos vinculados a uma marca.', join: 'INNER JOIN · VEÍCULOS + FABRICANTES', param: 'fabricanteId', label: 'Fabricante', source: 'fabricantes' },
  { key: 'alugueis-por-cliente', title: 'Aluguéis por cliente', description: 'Histórico de locações de um cliente com o veículo associado.', join: 'INNER JOIN · ALUGUÉIS + CLIENTES + VEÍCULOS', param: 'clienteId', label: 'Cliente', source: 'clientes' },
  { key: 'alugueis-abertos', title: 'Aluguéis abertos em uma data', description: 'Consulte quais locações estavam em andamento em um momento.', join: 'INNER JOIN · ALUGUÉIS + CLIENTES + VEÍCULOS', param: 'em', label: 'Data e hora', type: 'datetime-local' },
  { key: 'pagamentos-por-cliente', title: 'Pagamentos por cliente', description: 'Veja os pagamentos vinculados aos aluguéis de um cliente.', join: 'INNER JOIN · PAGAMENTOS + ALUGUÉIS + CLIENTES', param: 'clienteId', label: 'Cliente', source: 'clientes' },
  { key: 'veiculos-sem-aluguel', title: 'Veículos nunca alugados', description: 'Identifique veículos sem histórico de locação. Fabricante é opcional.', join: 'LEFT JOIN · VEÍCULOS + ALUGUÉIS', param: 'fabricanteId', label: 'Fabricante (opcional)', source: 'fabricantes', optional: true }
];

const $ = selector => document.querySelector(selector);
const esc = value => String(value ?? '').replace(/[&<>"']/g, char => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' }[char]));
const fmtMoney = value => value == null ? '—' : new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(Number(value));
const fmtDate = value => value ? new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value)) : '—';
const fmtKm = value => new Intl.NumberFormat('pt-BR').format(Number(value)) + ' km';
const normalize = value => String(value ?? '').normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase().trim();
const state = { page: 'inicio', resource: null, rows: [], selectedQuery: queries[0], editingId: null, mode: 'create', busy: false };
let toastTimer;

async function api(path, options = {}) {
  let response;
  try { response = await fetch('/api/' + path, { headers: { Accept: 'application/json', ...(options.body ? { 'Content-Type': 'application/json' } : {}) }, ...options }); }
  catch { throw new Error('Não foi possível conectar à API. Inicie o backend e confira o SQL Server Express.'); }
  if (response.status === 204) return null;
  const raw = await response.text();
  let body = null;
  try { body = raw ? JSON.parse(raw) : null; } catch { body = null; }
  if (!response.ok) {
    const validation = body?.errors && Object.values(body.errors).flat().join(' ');
    throw new Error(body?.erro || validation || body?.title || `A API retornou HTTP ${response.status}.`);
  }
  return body;
}

function toast(message, error = false) {
  const node = $('#toast'); node.textContent = message; node.classList.toggle('error', error); node.classList.add('show');
  clearTimeout(toastTimer); toastTimer = setTimeout(() => node.classList.remove('show'), 4200);
}
function setApiStatus(ok) {
  $('#api-dot').classList.toggle('ok', ok); $('#api-dot').classList.toggle('bad', !ok);
  $('#api-status').textContent = ok ? 'API conectada' : 'API indisponível';
}
function emptyHtml(title, description, error = false) {
  return `<div class="empty-state${error ? ' error-state' : ''}"><span>${error ? '!' : '▦'}</span><strong>${esc(title)}</strong><p>${esc(description)}</p></div>`;
}
function formatCell(row, col) {
  const value = row[col.key];
  if (col.format === 'money') return esc(fmtMoney(value));
  if (col.format === 'date') return esc(fmtDate(value));
  if (col.format === 'km') return esc(fmtKm(value));
  if (col.format === 'cpf') return esc(String(value ?? '').replace(/^(\d{3})(\d{3})(\d{3})(\d{2})$/, '$1.$2.$3-$4'));
  if (col.format === 'status') return value ? '<span class="badge badge-done">Devolvido</span>' : '<span class="badge badge-open">Em aberto</span>';
  return esc(value ?? '—');
}

function navigate(page) {
  if (page !== 'inicio' && page !== 'consultas' && !resources[page]) return;
  state.page = page; state.resource = resources[page] ? page : null;
  document.querySelectorAll('.page').forEach(el => el.classList.remove('active'));
  $('#page-' + (state.resource ? 'resource' : page)).classList.add('active');
  document.querySelectorAll('.nav-item').forEach(el => el.classList.toggle('active', el.dataset.page === page));
  const title = resources[page]?.title || (page === 'consultas' ? 'Consultas' : 'Visão geral');
  $('#crumb').textContent = title; document.title = `${title} | Locadora Atlas`;
  history.replaceState(null, '', '#' + page);
  if (state.resource) showResource(page);
  else if (page === 'inicio') loadHome();
  else showQueries();
  window.scrollTo({ top: 0, behavior: 'smooth' });
}

async function loadHome() {
  const keys = ['veiculos', 'clientes', 'alugueis', 'pagamentos'];
  try {
    const [veiculos, clientes, alugueis, pagamentos] = await Promise.all(keys.map(key => api(key)));
    const data = { veiculos, clientes, alugueis, pagamentos };
    keys.forEach(key => { $('#metric-' + key).textContent = data[key].length; });
    $('#metric-alugueis').textContent = alugueis.filter(x => !x.devolucao).length;
    $('#recent-rentals').innerHTML = alugueis.slice(-4).reverse().map(a => `<div class="recent-item"><div><strong>${esc(a.cliente)}</strong><small>${esc(a.veiculo)} · ${esc(fmtDate(a.inicio))}</small></div><span class="badge ${a.devolucao ? 'badge-done' : 'badge-open'}">${a.devolucao ? 'Devolvido' : 'Em aberto'}</span></div>`).join('') || '<p class="muted">Nenhum aluguel cadastrado.</p>';
    setApiStatus(true);
  } catch (error) {
    keys.forEach(key => { $('#metric-' + key).textContent = '—'; });
    $('#recent-rentals').innerHTML = emptyHtml('Sem conexão', error.message, true);
    setApiStatus(false);
  }
}

function showResource(key) {
  const cfg = resources[key];
  $('#resource-title').textContent = cfg.title;
  $('#resource-description').textContent = cfg.description;
  $('#resource-eyebrow').textContent = key === 'alugueis' || key === 'pagamentos' ? 'OPERAÇÃO' : 'CADASTROS';
  $('#add-button').textContent = '+ Novo ' + cfg.singular;
  $('#resource-filters').innerHTML = cfg.filters.map((f, i) => `<div class="field"><label for="filter-${i}">${esc(f.label)}</label><input id="filter-${i}" type="search" placeholder="${esc(f.placeholder)}" autocomplete="off"></div>`).join('');
  $('#resource-head').innerHTML = '<tr>' + cfg.columns.map(c => `<th scope="col">${esc(c.label)}</th>`).join('') + '<th scope="col">Ações</th></tr>';
  $('#resource-body').innerHTML = `<tr><td colspan="${cfg.columns.length + 1}">Carregando…</td></tr>`;
  $('#resource-foot').textContent = '';
  $('#resource-filters').querySelectorAll('input').forEach(input => input.addEventListener('input', renderRows));
  loadResource(key);
}

async function loadResource(key) {
  try {
    const rows = await api(key);
    if (state.resource !== key) return;
    state.rows = rows; renderRows(); setApiStatus(true);
  } catch (error) {
    if (state.resource !== key) return;
    state.rows = [];
    $('#resource-body').innerHTML = `<tr><td colspan="${resources[key].columns.length + 1}">${emptyHtml('Não foi possível carregar os dados', error.message, true)}</td></tr>`;
    $('#resource-foot').textContent = 'Verifique a conexão e recarregue a página.';
    setApiStatus(false);
  }
}

function renderRows() {
  const cfg = resources[state.resource]; if (!cfg) return;
  const filters = cfg.filters.map((f, i) => normalize($('#filter-' + i)?.value));
  const rows = state.rows.filter(row => cfg.filters.every((f, i) => normalize(row[f.key]).includes(filters[i])));
  $('#resource-body').innerHTML = rows.length ? rows.map(row => `<tr>${cfg.columns.map(c => `<td>${formatCell(row, c)}</td>`).join('')}<td><div class="row-actions"><button class="action-btn" type="button" data-action="view" data-id="${Number(row.id)}">Ver</button><button class="action-btn" type="button" data-action="edit" data-id="${Number(row.id)}">Editar</button>${state.resource === 'alugueis' && !row.devolucao ? `<button class="action-btn" type="button" data-action="return" data-id="${Number(row.id)}">Devolver</button>` : ''}<button class="action-btn danger" type="button" data-action="delete" data-id="${Number(row.id)}">Excluir</button></div></td></tr>`).join('') : `<tr><td colspan="${cfg.columns.length + 1}">${emptyHtml('Nenhum resultado', 'Ajuste os filtros ou crie um novo registro.')}</td></tr>`;
  $('#resource-foot').textContent = `${rows.length} de ${state.rows.length} registro${state.rows.length === 1 ? '' : 's'} exibido${rows.length === 1 ? '' : 's'}`;
}

async function optionsFor(source) {
  const rows = await api(source);
  return rows.filter(x => source !== 'alugueis' || !!x.devolucao).map(x => ({ value: x.id, label: source === 'fabricantes' ? x.nome : source === 'clientes' ? `${x.nome} · ${x.cpf}` : source === 'veiculos' ? `${x.modelo} · ${x.placa}` : `#${x.id} · ${x.cliente} · ${x.veiculo}` }));
}
function fieldHtml(field, value, choices, readonly) {
  const id = 'form-' + field.key; const v = value ?? '';
  const attrs = `${field.required ? 'required' : ''} ${field.max ? `maxlength="${field.max}"` : ''} ${field.min !== undefined ? `min="${field.min}"` : ''} ${field.step ? `step="${field.step}"` : ''} ${field.pattern ? `pattern="${esc(field.pattern)}"` : ''} ${readonly ? 'disabled' : ''}`;
  const hint = field.hint ? `<small>${esc(field.hint)}</small>` : '';
  if (field.type === 'select' || field.type === 'choice') {
    const options = field.type === 'choice' ? field.options.map(x => ({ value: x, label: x })) : choices[field.source] || [];
    return `<div class="field ${field.full ? 'full' : ''}"><label for="${id}">${esc(field.label)}</label><select id="${id}" name="${field.key}" ${attrs}><option value="">Selecione…</option>${options.map(x => `<option value="${esc(x.value)}" ${String(x.value) === String(v) ? 'selected' : ''}>${esc(x.label)}</option>`).join('')}</select>${hint}</div>`;
  }
  const actual = field.type === 'datetime-local' && v ? String(v).slice(0, 16) : v;
  return `<div class="field ${field.full ? 'full' : ''}"><label for="${id}">${esc(field.label)}</label><input id="${id}" name="${field.key}" type="${field.type || 'text'}" value="${esc(actual)}" ${attrs}>${hint}</div>`;
}

async function openForm(mode, id = null) {
  const resource = state.resource; const cfg = resources[resource];
  state.mode = mode; state.editingId = id;
  const dialog = $('#record-dialog'); const error = $('#form-error'); error.hidden = true;
  $('#dialog-title').textContent = mode === 'return' ? 'Registrar devolução' : mode === 'view' ? `Detalhes do ${cfg.singular}` : mode === 'edit' ? `Editar ${cfg.singular}` : `Novo ${cfg.singular}`;
  $('#dialog-submit').hidden = mode === 'view';
  $('#form-fields').innerHTML = '<p class="muted">Carregando dados…</p>';
  dialog.showModal();
  try {
    const record = id != null ? await api(`${resource}/${id}`) : {};
    if (!dialog.open || state.resource !== resource) return;
    const fields = mode === 'return' ? [{ key: 'devolucao', label: 'Data e hora da devolução', type: 'datetime-local', required: true }, { key: 'quilometragemFinal', label: 'Quilometragem final', type: 'number', min: 0, required: true }] : cfg.fields;
    const sources = [...new Set(fields.map(f => f.source).filter(Boolean))];
    const entries = await Promise.all(sources.map(async source => [source, await optionsFor(source)]));
    const choices = Object.fromEntries(entries);
    if (!dialog.open || state.resource !== resource) return;
    $('#form-fields').innerHTML = fields.map(f => fieldHtml(f, mode === 'return' ? (f.key === 'quilometragemFinal' ? record.quilometragemInicial : '') : record[f.key], choices, mode === 'view' || (mode === 'edit' && resource === 'alugueis' && f.key === 'veiculoId'))).join('');
    if (mode === 'return') $('#form-fields').insertAdjacentHTML('afterbegin', `<div class="field full"><label>Aluguel #${Number(id)}</label><small>Quilometragem inicial: ${esc(fmtKm(record.quilometragemInicial))}. A diária é ${esc(fmtMoney(record.valorDiaria))}.</small></div>`);
  } catch (e) { $('#form-fields').innerHTML = ''; error.textContent = e.message; error.hidden = false; }
}

async function submitForm(event) {
  event.preventDefault(); if (state.busy) return;
  const form = $('#record-form'); const error = $('#form-error'); error.hidden = true;
  if (!form.reportValidity()) return;
  const resource = state.resource; const cfg = resources[resource];
  const fields = state.mode === 'return' ? [{ key: 'devolucao', type: 'datetime-local' }, { key: 'quilometragemFinal', type: 'number' }] : cfg.fields;
  const payload = {};
  for (const field of fields) {
    let value = form.elements.namedItem(field.key)?.value;
    if (field.type === 'number' || field.type === 'select') value = Number(value);
    else if (field.key === 'placa') value = value.toUpperCase();
    payload[field.key] = value;
  }
  if (resource === 'alugueis' && state.mode === 'edit') payload.veiculoId = Number(form.elements.namedItem('veiculoId').value);
  const method = state.mode === 'create' ? 'POST' : 'PUT';
  const path = resource + (state.mode === 'create' ? '' : `/${state.editingId}`) + (state.mode === 'return' ? '/devolucao' : '');
  state.busy = true; $('#dialog-submit').disabled = true;
  try {
    const result = await api(path, { method, body: JSON.stringify(payload) });
    $('#record-dialog').close();
    toast(state.mode === 'return' ? `Devolução registrada. Total: ${fmtMoney(result.valorTotal)}` : state.mode === 'create' ? 'Registro criado com sucesso.' : 'Registro atualizado com sucesso.');
    await loadResource(resource);
  } catch (e) { error.textContent = e.message; error.hidden = false; }
  finally { state.busy = false; $('#dialog-submit').disabled = false; }
}

function confirmDelete(id) {
  const row = state.rows.find(x => x.id === id);
  $('#confirm-message').textContent = `Você está prestes a excluir ${resources[state.resource].singular} #${id}${row?.nome ? ` (${row.nome})` : ''}. Registros com histórico vinculado são protegidos pelo backend.`;
  $('#confirm-yes').dataset.id = id;
  $('#confirm-dialog').showModal();
}
async function deleteRecord() {
  const id = Number($('#confirm-yes').dataset.id); const resource = state.resource;
  $('#confirm-yes').disabled = true;
  try { await api(`${resource}/${id}`, { method: 'DELETE' }); $('#confirm-dialog').close(); toast('Registro excluído com sucesso.'); await loadResource(resource); }
  catch (e) { $('#confirm-dialog').close(); toast(e.message, true); }
  finally { $('#confirm-yes').disabled = false; }
}

function showQueries() {
  $('#query-menu').innerHTML = queries.map((q, i) => `<button type="button" class="query-choice ${q.key === state.selectedQuery.key ? 'active' : ''}" data-query="${esc(q.key)}"><span>${i + 1}</span>${esc(q.title)}</button>`).join('');
  selectQuery(state.selectedQuery.key);
}
async function selectQuery(key) {
  const q = queries.find(x => x.key === key); if (!q) return; state.selectedQuery = q;
  document.querySelectorAll('.query-choice').forEach(el => el.classList.toggle('active', el.dataset.query === key));
  $('#query-title').textContent = q.title; $('#query-join').textContent = q.join; $('#query-description').textContent = q.description;
  $('#query-result').innerHTML = emptyHtml('Pronto para consultar', 'Defina o filtro e execute a busca.');
  $('#query-form').innerHTML = '<p class="muted">Carregando parâmetros…</p>';
  try {
    const choices = q.source ? await optionsFor(q.source) : [];
    if (state.selectedQuery.key !== key) return;
    const input = q.source ? `<select id="query-param" name="${q.param}" ${q.optional ? '' : 'required'}><option value="">${q.optional ? 'Todos os fabricantes' : 'Selecione…'}</option>${choices.map(x => `<option value="${esc(x.value)}">${esc(x.label)}</option>`).join('')}</select>` : `<input id="query-param" name="${q.param}" type="datetime-local" required>`;
    $('#query-form').innerHTML = `<div class="field"><label for="query-param">${esc(q.label)}</label>${input}</div><button type="submit" class="btn btn-primary">Executar consulta</button>`;
  } catch (e) { $('#query-form').innerHTML = emptyHtml('Parâmetros indisponíveis', e.message, true); setApiStatus(false); }
}
async function executeQuery(event) {
  event.preventDefault(); const q = state.selectedQuery; const form = $('#query-form');
  if (!form.reportValidity()) return;
  const value = $('#query-param').value;
  const params = new URLSearchParams(); if (value) params.set(q.param, value);
  $('#query-result').innerHTML = '<p class="query-result-label">Consultando a API…</p>';
  try {
    const result = await api(`consultas/${q.key}${params.size ? '?' + params : ''}`);
    if (state.selectedQuery.key !== q.key) return;
    setApiStatus(true);
    if (!result.length) { $('#query-result').innerHTML = emptyHtml('Nenhum resultado', 'A consulta foi executada, mas não encontrou registros.'); return; }
    const columns = Object.keys(result[0]);
    const cells = item => columns.map(key => `<td>${esc(key.toLowerCase().includes('data') || ['inicio', 'fimPrevisto', 'devolucao'].includes(key) ? fmtDate(item[key]) : key.toLowerCase().includes('valor') ? fmtMoney(item[key]) : item[key] ?? '—')}</td>`).join('');
    $('#query-result').innerHTML = `<div class="query-result-label">${result.length} resultado${result.length === 1 ? '' : 's'} encontrado${result.length === 1 ? '' : 's'}</div><div class="table-wrap"><table><thead><tr>${columns.map(c => `<th scope="col">${esc(c.replace(/([A-Z])/g, ' $1'))}</th>`).join('')}</tr></thead><tbody>${result.map(r => `<tr>${cells(r)}</tr>`).join('')}</tbody></table></div>`;
  } catch (e) { $('#query-result').innerHTML = emptyHtml('Falha na consulta', e.message, true); setApiStatus(false); }
}

$('#main-nav').addEventListener('click', e => { const button = e.target.closest('[data-page]'); if (button) navigate(button.dataset.page); });
document.querySelectorAll('[data-page="consultas"]').forEach(button => button.addEventListener('click', () => navigate('consultas')));
document.querySelectorAll('[data-jump]').forEach(button => button.addEventListener('click', () => navigate(button.dataset.jump)));
$('#refresh-home').addEventListener('click', loadHome);
$('#add-button').addEventListener('click', () => openForm('create'));
$('#clear-filters').addEventListener('click', () => { $('#resource-filters').querySelectorAll('input').forEach(input => { input.value = ''; }); renderRows(); });
$('#resource-body').addEventListener('click', e => {
  const button = e.target.closest('[data-action]'); if (!button) return;
  const id = Number(button.dataset.id);
  if (button.dataset.action === 'delete') confirmDelete(id);
  else openForm(button.dataset.action, id);
});
$('#record-form').addEventListener('submit', submitForm);
$('#dialog-close').addEventListener('click', () => $('#record-dialog').close());
$('#dialog-cancel').addEventListener('click', () => $('#record-dialog').close());
$('#confirm-cancel').addEventListener('click', () => $('#confirm-dialog').close());
$('#confirm-yes').addEventListener('click', deleteRecord);
$('#query-menu').addEventListener('click', e => { const button = e.target.closest('[data-query]'); if (button) selectQuery(button.dataset.query); });
$('#query-form').addEventListener('submit', executeQuery);
$('#today').textContent = new Intl.DateTimeFormat('pt-BR', { dateStyle: 'long' }).format(new Date());
const initial = location.hash.slice(1);
navigate(initial === 'consultas' || resources[initial] ? initial : 'inicio');
