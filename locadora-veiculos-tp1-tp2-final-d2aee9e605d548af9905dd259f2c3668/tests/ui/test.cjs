const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { JSDOM } = require('jsdom');
const html = fs.readFileSync(path.resolve(__dirname, '../../Locadora.Api/wwwroot/index.html'), 'utf8');
const js = fs.readFileSync(path.resolve(__dirname, '../../Locadora.Api/wwwroot/app.js'), 'utf8');
const dom = new JSDOM(html, { url: 'http://localhost:5080/', runScripts: 'outside-only' });
const { window } = dom;
window.scrollTo = () => {};
window.HTMLDialogElement.prototype.showModal = function () { this.setAttribute('open', ''); };
window.HTMLDialogElement.prototype.close = function () { this.removeAttribute('open'); };
const data = {
  fabricantes: [{ id: 1, nome: 'Volkswagen', pais: 'Alemanha' }],
  veiculos: [{ id: 2, fabricanteId: 1, fabricante: 'Volkswagen', modelo: 'Polo', anoFabricacao: 2022, quilometragem: 25000, placa: 'ABC1D23', valorDiaria: 150 }],
  clientes: [{ id: 3, nome: 'Ana', cpf: '52998224725', email: 'ana@example.com' }],
  alugueis: [{ id: 4, clienteId: 3, cliente: 'Ana', veiculoId: 2, veiculo: 'Polo', inicio: '2027-02-10T10:00:00', fimPrevisto: '2027-02-12T10:00:00', devolucao: null, quilometragemInicial: 25000, quilometragemFinal: null, valorDiaria: 150, valorTotal: null }],
  pagamentos: []
};
const calls = [];
window.fetch = async (url, options = {}) => {
  const method = options.method || 'GET'; calls.push({ url, method, body: options.body ? JSON.parse(options.body) : null });
  const key = url.replace('/api/', '').split(/[/?]/)[0];
  let status = method === 'POST' ? 201 : ['PUT', 'DELETE'].includes(method) ? 204 : 200;
  let body = null;
  if (url.startsWith('/api/consultas/')) body = url.includes('veiculos-por-fabricante') ? [{ id: 2, modelo: 'Polo', placa: 'ABC1D23', fabricante: 'Volkswagen' }] : [];
  else if (method === 'GET') body = url.match(/\/\d+$/) ? data[key].find(x => x.id === Number(url.split('/').at(-1))) : data[key];
  else if (method === 'POST') body = { id: 99, ...JSON.parse(options.body) };
  else if (url.endsWith('/devolucao')) body = { id: 4, valorTotal: 300 };
  return { ok: status >= 200 && status < 300, status, text: async () => body == null ? '' : JSON.stringify(body) };
};
const wait = () => new Promise(resolve => setTimeout(resolve, 15));
(async () => {
  window.eval(js); await wait();
  assert.equal(window.document.querySelector('#metric-veiculos').textContent, '1');
  assert.equal(window.document.querySelector('#metric-alugueis').textContent, '1');
  window.document.querySelector('[data-page="veiculos"]').click(); await wait();
  assert.match(window.document.querySelector('#resource-body').textContent, /Polo/);
  const filter = window.document.querySelector('#filter-0'); filter.value = 'outro'; filter.dispatchEvent(new window.Event('input', { bubbles: true }));
  assert.match(window.document.querySelector('#resource-body').textContent, /Nenhum resultado/);
  filter.value = 'Polo'; filter.dispatchEvent(new window.Event('input', { bubbles: true }));
  assert.match(window.document.querySelector('#resource-body').textContent, /Polo/);
  window.document.querySelector('#resource-body [data-action="edit"]').click(); await wait();
  assert.equal(window.document.querySelector('#form-modelo').value, 'Polo');
  assert.equal(window.document.querySelector('#form-fabricanteId').value, '1');
  window.document.querySelector('#record-dialog').close();
  window.document.querySelector('[data-page="fabricantes"]').click(); await wait();
  window.document.querySelector('#add-button').click(); await wait();
  window.document.querySelector('#form-nome').value = 'Fiat';
  window.document.querySelector('#form-pais').value = 'Itália';
  window.document.querySelector('#record-form').dispatchEvent(new window.Event('submit', { bubbles: true, cancelable: true })); await wait();
  assert(calls.some(c => c.url === '/api/fabricantes' && c.method === 'POST' && c.body.nome === 'Fiat'));
  window.document.querySelector('[data-page="alugueis"]').click(); await wait();
  window.document.querySelector('#resource-body [data-action="return"]').click(); await wait();
  assert.equal(window.document.querySelector('#form-quilometragemFinal').value, '25000');
  window.document.querySelector('#record-dialog').close();
  window.document.querySelector('[data-page="consultas"]').click(); await wait();
  window.document.querySelector('#query-param').value = '1';
  window.document.querySelector('#query-form').dispatchEvent(new window.Event('submit', { bubbles: true, cancelable: true })); await wait();
  assert.match(window.document.querySelector('#query-result').textContent, /Polo/);
  assert(calls.some(c => c.url === '/api/consultas/veiculos-por-fabricante?fabricanteId=1'));
  console.log('Interface: 7 verificações funcionais aprovadas com respostas controladas da API.');
})().catch(e => { console.error(e); process.exitCode = 1; });
